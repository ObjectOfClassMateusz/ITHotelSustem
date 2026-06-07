using HotelSystemIndustry.Controllers;
using HotelSystemIndustry.Infrastructure;
using HotelSystemIndustry.Models;
using HotelSystemIndustry.Models.HousekeepingMaintenance;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HotelSystemIndustry.Controllers.Maintenance
{
    [Authorize(Roles = "MaintenanceEmployee")]
    public class MaintenanceController : Controller
    {
        private readonly HotelDbContext _context;
        private readonly UserManager<User> _userManager;

        public MaintenanceController(HotelDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private async Task<Guid> GetCurrentHotelId()
        {
            HotelChangeController hotelChangeController = new HotelChangeController(_context)
            {
                ControllerContext = this.ControllerContext
            };
            return await hotelChangeController.GetCurrentHotel();
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            Guid currentHotelId = await GetCurrentHotelId();

            var pendingRequests = await _context.MaintenanceRequests
                .Include(mr => mr.Room)
                .Where(mr => mr.Room.HotelId == currentHotelId &&
                             mr.Status == MaintenanceStatus.AWAITING_DECISION)
                .OrderByDescending(mr => mr.Priority).Take(5).ToListAsync();

            var lowSupplies = await _context.HousekeepingSupplies
                .Where(s => s.HotelId == currentHotelId &&
                            s.QuantityInStock < s.MinimumRequiredQuantity).ToListAsync();

            ViewBag.PendingRequests = pendingRequests;
            ViewBag.LowSupplies = lowSupplies;
            ViewBag.HotelChangePartialHotelList = new SelectList(_context.Hotels, "Id", "Name", currentHotelId);
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> CleaningList()
        {
            Guid currentHotelId = await GetCurrentHotelId();

            var cleanings = await _context.RoomCleanings
                .Include(rc => rc.Room)
                .Where(rc => rc.Room.HotelId == currentHotelId)
                .OrderByDescending(rc => rc.ScheduledDate).ToListAsync();
            return View(cleanings);
        }

        [HttpGet]
        public async Task<IActionResult> ScheduleCleaning()
        {
            Guid currentHotelId = await GetCurrentHotelId();

            var rooms = await _context.Rooms
                .Where(r => r.HotelId == currentHotelId)
                .AsNoTracking().ToListAsync();

            var activeShifts = await _context.EmployeeShifts
                .Where(s => s.HotelId == currentHotelId && s.EndTime > DateTime.UtcNow)
                .OrderBy(s => s.StartTime)
                .ToListAsync();

            ViewBag.RoomsSelectList = new SelectList(rooms, "Id", "RoomNumber");
            ViewBag.ShiftsSelectList = new SelectList(
                activeShifts.Select(s => new {
                    s.EmployeeEmail,
                    Display = s.EmployeeName + " (" + s.StartTime.ToLocalTime().ToString("dd.MM HH:mm") + " - " + s.EndTime.ToLocalTime().ToString("HH:mm") + ")"
                }), "EmployeeEmail", "Display");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ScheduleCleaning(Guid roomId, string employeeEmail, DateTime scheduledDate)
        {
            Guid currentHotelId = await GetCurrentHotelId();

            var room = await _context.Rooms.FindAsync(roomId);
            if (room == null) return NotFound();

            var user = await _userManager.FindByEmailAsync(employeeEmail);
            if (user == null) return NotFound();

            var scheduledUtc = DateTime.SpecifyKind(scheduledDate, DateTimeKind.Local).ToUniversalTime();

            var hasShift = await _context.EmployeeShifts
                .AnyAsync(s => s.EmployeeEmail == employeeEmail &&
                               s.HotelId == currentHotelId &&
                               s.StartTime <= scheduledUtc &&
                               s.EndTime >= scheduledUtc);

            if (!hasShift)
            {
                ModelState.AddModelError("", "Pracownik nie ma zmiany w tym czasie.");
                var rooms = await _context.Rooms
                    .Where(r => r.HotelId == currentHotelId)
                    .AsNoTracking().ToListAsync();
                var activeShifts = await _context.EmployeeShifts
                    .Where(s => s.HotelId == currentHotelId && s.EndTime > DateTime.UtcNow)
                    .ToListAsync();
                ViewBag.RoomsSelectList = new SelectList(rooms, "Id", "RoomNumber");
                ViewBag.ShiftsSelectList = new SelectList(
                    activeShifts.Select(s => new {
                        s.EmployeeEmail,
                        Display = s.EmployeeName + " (" + s.StartTime.ToLocalTime().ToString("dd.MM HH:mm") + " - " + s.EndTime.ToLocalTime().ToString("HH:mm") + ")"
                    }), "EmployeeEmail", "Display");
                return View();
            }

            var cleaning = new RoomCleaning
            {
                Id = Guid.NewGuid(),
                RoomId = roomId,
                Room = room,
                AssignedEmployeeName = user.FullName,
                AssignedEmployeeEmail = employeeEmail,
                ScheduledDate = scheduledUtc,
                Status = CleaningStatus.SCHEDULED
            };

            _context.RoomCleanings.Add(cleaning);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(CleaningList));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCleaning(Guid id)
        {
            var cleaning = await _context.RoomCleanings
                .Include(rc => rc.SupplyUsages)
                .FirstOrDefaultAsync(rc => rc.Id == id);
            if (cleaning == null) return NotFound();

            if (cleaning.SupplyUsages != null && cleaning.SupplyUsages.Any())
                _context.SupplyUsages.RemoveRange(cleaning.SupplyUsages);

            _context.RoomCleanings.Remove(cleaning);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(CleaningList));
        }

        [HttpGet]
        public async Task<IActionResult> MaintenanceList(string? status)
        {
            Guid currentHotelId = await GetCurrentHotelId();

            var query = _context.MaintenanceRequests
                .Include(mr => mr.Room)
                .Where(mr => mr.Room.HotelId == currentHotelId)
                .OrderByDescending(mr => mr.Priority)
                .ThenBy(mr => mr.ReportedDate);

            List<MaintenanceRequest> requests;
            if (!string.IsNullOrEmpty(status) && Enum.TryParse<MaintenanceStatus>(status, out var parsedStatus))
                requests = await query.Where(mr => mr.Status == parsedStatus).ToListAsync();
            else
                requests = await query.ToListAsync();

            ViewBag.CurrentStatus = status;
            return View(requests);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateMaintenanceStatus(Guid id, MaintenanceStatus status)
        {
            var request = await _context.MaintenanceRequests.FindAsync(id);
            if (request == null) return NotFound();

            request.Status = status;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(MaintenanceList));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMaintenance(Guid id)
        {
            var request = await _context.MaintenanceRequests.FindAsync(id);
            if (request == null) return NotFound();

            request.Status = MaintenanceStatus.RESOLVED;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(MaintenanceList));
        }

        [HttpGet]
        public async Task<IActionResult> SupplyList()
        {
            Guid currentHotelId = await GetCurrentHotelId();

            var supplies = await _context.HousekeepingSupplies
                .Where(s => s.HotelId == currentHotelId)
                .AsNoTracking().OrderBy(s => s.Name).ToListAsync();
            return View(supplies);
        }

        [HttpGet]
        public async Task<IActionResult> CreateSupply()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSupply(HousekeepingSupply supply)
        {
            ModelState.Remove("Hotel");
            if (!ModelState.IsValid) return View(supply);

            Guid currentHotelId = await GetCurrentHotelId();
            supply.Id = Guid.NewGuid();
            supply.HotelId = currentHotelId;
            _context.HousekeepingSupplies.Add(supply);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(SupplyList));
        }

        [HttpGet]
        public async Task<IActionResult> EditSupply(Guid id)
        {
            var supply = await _context.HousekeepingSupplies.FindAsync(id);
            if (supply == null) return NotFound();
            return View(supply);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSupply(HousekeepingSupply supply)
        {
            if (!ModelState.IsValid) return View(supply);

            _context.HousekeepingSupplies.Update(supply);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(SupplyList));
        }

        [HttpGet]
        public async Task<IActionResult> LostAndFoundList()
        {
            Guid currentHotelId = await GetCurrentHotelId();

            var items = await _context.LostAndFoundItems
                .Include(i => i.Room)
                .Where(i => i.Room.HotelId == currentHotelId)
                .OrderByDescending(i => i.FoundDate).ToListAsync();
            return View(items);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateLostItemStatus(Guid id, LostAndFoundStatus status)
        {
            var item = await _context.LostAndFoundItems.FindAsync(id);
            if (item == null) return NotFound();

            item.Status = status;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(LostAndFoundList));
        }

        [HttpGet]
        public async Task<IActionResult> ShiftList()
        {
            Guid currentHotelId = await GetCurrentHotelId();

            var shifts = await _context.EmployeeShifts
                .Where(s => s.HotelId == currentHotelId)
                .AsNoTracking().OrderByDescending(s => s.StartTime).ToListAsync();
            return View(shifts);
        }

        [HttpGet]
        public async Task<IActionResult> CreateShift()
        {
            var housekeepingUsers = await _userManager.GetUsersInRoleAsync("HousekeepingEmployee");
            ViewBag.EmployeesSelectList = new SelectList(
                housekeepingUsers.Select(u => new { u.Email, Name = u.FullName }), "Email", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateShift(string employeeEmail, DateTime startTime, DateTime endTime)
        {
            if (string.IsNullOrEmpty(employeeEmail) || endTime <= startTime)
            {
                var housekeepingUsers = await _userManager.GetUsersInRoleAsync("HousekeepingEmployee");
                ViewBag.EmployeesSelectList = new SelectList(
                    housekeepingUsers.Select(u => new { u.Email, Name = u.FullName }), "Email", "Name");
                return View();
            }

            Guid currentHotelId = await GetCurrentHotelId();

            var user = await _userManager.FindByEmailAsync(employeeEmail);
            if (user == null) return NotFound();

            var shift = new EmployeeShift
            {
                Id = Guid.NewGuid(),
                EmployeeEmail = employeeEmail,
                EmployeeName = user.FullName,
                HotelId = currentHotelId,
                StartTime = DateTime.SpecifyKind(startTime, DateTimeKind.Local).ToUniversalTime(),
                EndTime = DateTime.SpecifyKind(endTime, DateTimeKind.Local).ToUniversalTime()
            };

            _context.EmployeeShifts.Add(shift);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ShiftList));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteShift(Guid id)
        {
            var shift = await _context.EmployeeShifts.FindAsync(id);
            if (shift == null) return NotFound();

            _context.EmployeeShifts.Remove(shift);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ShiftList));
        }
    }
}