using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HotelSystemIndustry.Infrastructure;
using HotelSystemIndustry.Models.Events;
using Microsoft.AspNetCore.Authorization;

namespace HotelSystemIndustry.Controllers.Events
{
    [Authorize(Roles="HotelEmployee,Admin")]
    public class EventReservationController : Controller
    {
        private readonly HotelDbContext _context;

        public EventReservationController(HotelDbContext context)
        {
            _context = context;
        }

        // GET: EventReservation
        public async Task<IActionResult> Index()
        {
            var hotelDbContext = _context.EventReservations.Include(e => e.EventType).Include(e => e.Status);
            return View(await hotelDbContext.ToListAsync());
        }

        // GET: EventReservation/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventReservation = await _context.EventReservations
                .Include(e => e.EventType)
                .Include(e => e.Status)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (eventReservation == null)
            {
                return NotFound();
            }

            return View(eventReservation);
        }


        // GET: EventReservation/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventReservation = await _context.EventReservations
                .Include(e => e.EventType)
                .Include(e => e.Status)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (eventReservation == null)
            {
                return NotFound();
            }

            return View(eventReservation);
        }

        // POST: EventReservation/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var eventReservation = await _context.EventReservations.FindAsync(id);
            if (eventReservation != null)
            {
                _context.EventReservations.Remove(eventReservation);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
