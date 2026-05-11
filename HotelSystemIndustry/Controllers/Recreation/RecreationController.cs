using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelSystemIndustry.Infrastructure;
using HotelSystemIndustry.Models.Recreation;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace HotelSystemIndustry.Controllers.Recreation
{
    // [Authorize(Roles="Receptionist")] 
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
            var facilities = await _context.RecreationFacilities
                .AsNoTracking()
                .ToListAsync();

            return View(facilities);
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

            ViewBag.FacilityName = facility.Name;
            ViewBag.FacilityId = facility.Id;
            ViewBag.GuestsSelectList = new SelectList(await _context.Guests.AsNoTracking().ToListAsync(), "Id", "LastName");

            return View(new RecreationBooking { FacilityId = facilityId.Value, Facility = facility, Guest = null! });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Book([Bind("GuestId,FacilityId,StartTime,EndTime")] RecreationBooking booking)
        {
            if (ModelState.IsValid)
            {
                booking.Id = Guid.NewGuid();
                booking.Status = BookingStatus.SCHEDULED;

                _context.RecreationBookings.Add(booking);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewBag.GuestsSelectList = new SelectList(await _context.Guests.AsNoTracking().ToListAsync(), "Id", "LastName", booking.GuestId);
            return View(booking);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelBooking(Guid id)
        {
            var booking = await _context.RecreationBookings.FindAsync(id);

            if (booking == null) return NotFound();

            booking.Status = BookingStatus.CANCELLED;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}