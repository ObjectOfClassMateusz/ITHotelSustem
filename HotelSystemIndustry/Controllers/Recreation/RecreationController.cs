using HotelSystemIndustry.Infrastructure;
using HotelSystemIndustry.Models;
using HotelSystemIndustry.Models.Recreation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HotelSystemIndustry.Controllers.Recreation
{
    [Authorize(Roles = "RecreationEmployee")]
    public class RecreationController : Controller
    {
        private readonly HotelDbContext _context;

        public RecreationController(HotelDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return NotFound();

            var facility = await _context.RecreationFacilities
                .AsNoTracking()
                .Include(f => f.Bookings)
                .ThenInclude(b => b.Guest)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (facility == null) return NotFound();

            return View(facility);
        }

        [HttpGet]
        public async Task<IActionResult> Book(Guid? facilityId)
        {
            if (facilityId == null) return NotFound();

            var facility = await _context.RecreationFacilities.FindAsync(facilityId);
            if (facility == null) return NotFound();

            // do testów -> dodanie gościa
            if (!await _context.Guests.AnyAsync())
            {
                _context.Guests.Add(new Guest { Id = Guid.NewGuid(), FirstName = "Jan", LastName = "Kowalski"});
                await _context.SaveChangesAsync();
            }

            ViewBag.FacilityName = facility.Name;
            ViewBag.FacilityId = facility.Id;
            var guestsList = await _context.Guests.AsNoTracking().Select(g => new {
                                                                 Id = g.Id,
                                                                 FullName = g.FirstName + " " + g.LastName}).ToListAsync();

            ViewBag.GuestsSelectList = new SelectList(guestsList,"Id","FullName");

            var bookingModel = new RecreationBooking
            {
                FacilityId = facilityId.Value,
                Facility = facility,
                Guest = null!,
                StartTime = DateTime.Today.AddHours(12),
                EndTime = DateTime.Today.AddHours(13)
            };

            return View(bookingModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Book([Bind("GuestId,FacilityId,StartTime,EndTime")] RecreationBooking booking)
        {
            ModelState.Remove("Facility");
            ModelState.Remove("Guest");

            booking.StartTime = DateTime.SpecifyKind(booking.StartTime, DateTimeKind.Utc);
            booking.EndTime = DateTime.SpecifyKind(booking.EndTime, DateTimeKind.Utc);

            if (booking.StartTime < DateTime.UtcNow)
            {
                ModelState.AddModelError("StartTime","Nie można dokonać rezerwacji w przeszłości.");
            }

            if (booking.EndTime <= booking.StartTime)
            {
                ModelState.AddModelError("EndTime","Czas zakończenia musi być późniejszy niż czas rozpoczęcia.");
            }

            var facility = await _context.RecreationFacilities.FindAsync(booking.FacilityId);
            if (facility == null) return NotFound();

            if (ModelState.IsValid)
            {
                //sprawdzamy maxcapacity w danym czasie, za każdą nakładającą sie rezerwacje zwiększamy zmienną
                var overlappingBookingsCount = await _context.RecreationBookings
                    .Where(b => b.FacilityId == booking.FacilityId &&
                                b.Status == BookingStatus.SCHEDULED &&
                                b.StartTime < booking.EndTime &&
                                b.EndTime > booking.StartTime)
                    .CountAsync();

                if (overlappingBookingsCount + 1 > facility.MaxCapacity)
                {
                    ModelState.AddModelError(string.Empty,"Brak miejsc w wybranym przedziale czasowym.");
                }
            }

            if (ModelState.IsValid)
            {
                booking.Status = BookingStatus.SCHEDULED;
                _context.RecreationBookings.Add(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(FacilityList));
            }

            ViewBag.FacilityName = facility.Name;
            ViewBag.FacilityId = booking.FacilityId;

            var guestsList = await _context.Guests
                .AsNoTracking()
                .Select(g => new {Id = g.Id, FullName = g.FirstName + " " + g.LastName})
                .ToListAsync();

            ViewBag.GuestsSelectList = new SelectList(guestsList,"Id","FullName", booking.GuestId);

            return View(booking);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelBooking(Guid id, Guid facilityId)
        {
            var booking = await _context.RecreationBookings.FindAsync(id);

            if (booking == null) return NotFound();

            booking.Status = BookingStatus.CANCELLED;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(BookingList), new {facilityId = facilityId});
        }

        [HttpGet]
        public async Task<IActionResult> FacilityList()
        {
            var facilities = await _context.RecreationFacilities.AsNoTracking().ToListAsync();
            return View(facilities);
        }

        [HttpGet]
        public async Task<IActionResult> CreateFacility()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFacility(RecreationFacility facility)
        {
            if (ModelState.IsValid)
            {
                _context.RecreationFacilities.Add(facility);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(FacilityList));
            }

            return View(facility);
        }

        [HttpGet]
        public async Task<IActionResult> BookingList(Guid facilityId)
        {
            var facility = await _context.RecreationFacilities
                .AsNoTracking()
                .Include(f => f.Bookings)
                .ThenInclude(b => b.Guest)
                .FirstOrDefaultAsync(f => f.Id == facilityId);

            if (facility == null) return NotFound();

            ViewBag.FacilityName = facility.Name;
            ViewBag.FacilityId = facility.Id;

            var activeBookings = facility.Bookings
                .Where(b => b.Status == BookingStatus.SCHEDULED)
                .OrderBy(b => b.StartTime)
                .ToList();

            return View(activeBookings);
        }
    }
}