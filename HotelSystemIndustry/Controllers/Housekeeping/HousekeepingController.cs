using HotelSystemIndustry.Infrastructure;
using HotelSystemIndustry.Models;
using HotelSystemIndustry.Models.HousekeepingMaintenance;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HotelSystemIndustry.Controllers.Housekeeping
{
    [Authorize(Roles = "HousekeepingEmployee")]
    public class HousekeepingController : Controller
    {
        private readonly HotelDbContext _context;
        private readonly UserManager<User> _userManager;

        public HousekeepingController(HotelDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private async Task<User?> GetCurrentUser()
        {
            return await _userManager.GetUserAsync(User);
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await GetCurrentUser();

            var myCleanings = await _context.RoomCleanings
                .Include(rc => rc.Room)
                .Where(rc => rc.AssignedEmployeeEmail == user!.Email &&
                             rc.ScheduledDate.Date == DateTime.Now.ToUniversalTime().Date &&
                             rc.Status != CleaningStatus.COMPLETED)
                .OrderBy(rc => rc.ScheduledDate)
                .ToListAsync();

            ViewBag.EmployeeName = user?.FullName;
            ViewBag.MyCleanings = myCleanings;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> MyCleanings()
        {
            var user = await GetCurrentUser();

            var cleanings = await _context.RoomCleanings
                .Include(rc => rc.Room)
                .Where(rc => rc.AssignedEmployeeEmail == user!.Email)
                .OrderByDescending(rc => rc.ScheduledDate)
                .ToListAsync();

            return View(cleanings);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StartCleaning(Guid id)
        {
            var cleaning = await _context.RoomCleanings.FindAsync(id);
            if (cleaning == null) return NotFound();

            cleaning.Status = CleaningStatus.IN_PROGRESS;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(MyCleanings));
        }

        [HttpGet]
        public async Task<IActionResult> CompleteCleaning(Guid id)
        {
            var cleaning = await _context.RoomCleanings
                .Include(rc => rc.Room)
                .FirstOrDefaultAsync(rc => rc.Id == id);

            if (cleaning == null) return NotFound();

            var supplies = await _context.HousekeepingSupplies
                .AsNoTracking()
                .Where(s => s.QuantityInStock > 0)
                .ToListAsync();

            ViewBag.SuppliesSelectList = new SelectList(supplies, "Id", "Name");

            return View(cleaning);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteCleaning(Guid cleaningId, Guid supplyId, decimal amountUsed)
        {
            var cleaning = await _context.RoomCleanings
                .Include(rc => rc.Room)
                .FirstOrDefaultAsync(rc => rc.Id == cleaningId);

            var supply = await _context.HousekeepingSupplies.FindAsync(supplyId);

            if (cleaning == null || supply == null) return NotFound();

            var usage = new SupplyUsage
            {
                Id = Guid.NewGuid(),
                RoomCleaning = cleaning,
                Supply = supply,
                AmountUsed = amountUsed
            };

            supply.QuantityInStock -= amountUsed;
            cleaning.Status = CleaningStatus.COMPLETED;

            _context.SupplyUsages.Add(usage);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(MyCleanings));
        }

        [HttpGet]
        public async Task<IActionResult> ReportMaintenance()
        {
            var rooms = await _context.Rooms.AsNoTracking().ToListAsync();
            ViewBag.RoomsSelectList = new SelectList(rooms, "Id", "RoomNumber");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReportMaintenance(Guid roomId, string description, MaintenancePriority priority)
        {
            var room = await _context.Rooms.FindAsync(roomId);
            if (room == null) return NotFound();

            var request = new MaintenanceRequest
            {
                Id = Guid.NewGuid(),
                RoomId = roomId,
                Room = room,
                Description = description,
                Priority = priority,
                Status = MaintenanceStatus.AWAITING_DECISION,
                ReportedDate = DateTime.UtcNow
            };

            _context.MaintenanceRequests.Add(request);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> ReportLostItem()
        {
            var rooms = await _context.Rooms.AsNoTracking().ToListAsync();
            ViewBag.RoomsSelectList = new SelectList(rooms, "Id", "RoomNumber");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReportLostItem(Guid roomId, string name, string? description)
        {
            var user = await GetCurrentUser();
            var room = await _context.Rooms.FindAsync(roomId);

            if (room == null) return NotFound();

            var item = new LostAndFoundItem
            {
                Id = Guid.NewGuid(),
                Name = name,
                Description = description,
                FoundDate = DateTime.UtcNow,
                Status = LostAndFoundStatus.IN_STORAGE,
                RoomId = roomId,
                Room = room,
                FoundByEmployeeName = user?.FullName ?? user?.Email ?? ""
            };

            _context.LostAndFoundItems.Add(item);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}