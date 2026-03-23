using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HotelSystemIndustry.Infrastructure;
using HotelSystemIndustry.Models.Events;

namespace HotelSystemIndustry.Controllers.Events
{
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

        // GET: EventReservation/Create
        public IActionResult Create()
        {
            ViewData["EventTypeId"] = new SelectList(_context.EventTypes, "Id", "Name");
            ViewData["StatusId"] = new SelectList(_context.EventReservationStatuses, "Id", "Name");
            return View();
        }

        // POST: EventReservation/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,StatusId,EventTypeId,StartTime,EndTime,NumRequiredStaff,NumGuests")] EventReservation eventReservation)
        {
            if (ModelState.IsValid &&
                eventReservation.EventTypeId != Guid.Empty &&
                eventReservation.StatusId != Guid.Empty)
            {
                eventReservation.EventType = _context.EventTypes.Where(et => et.Id == eventReservation.EventTypeId).Single();
                eventReservation.Status = _context.EventReservationStatuses.Where(s => s.Id == eventReservation.StatusId).Single();

                eventReservation.StartTime = eventReservation.StartTime.ToUniversalTime();
                eventReservation.EndTime = eventReservation.EndTime.ToUniversalTime();

                eventReservation.Id = Guid.NewGuid();
                _context.Add(eventReservation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EventTypeId"] = new SelectList(_context.EventTypes, "Id", "Name", eventReservation.EventTypeId);
            ViewData["StatusId"] = new SelectList(_context.EventReservationStatuses, "Id", "Name", eventReservation.StatusId);
            return View(eventReservation);
        }

        // GET: EventReservation/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventReservation = await _context.EventReservations.FindAsync(id);
            if (eventReservation == null)
            {
                return NotFound();
            }
            ViewData["EventTypeId"] = new SelectList(_context.EventTypes, "Id", "Name", eventReservation.EventTypeId);
            ViewData["StatusId"] = new SelectList(_context.EventReservationStatuses, "Id", "Name", eventReservation.StatusId);
            return View(eventReservation);
        }

        // POST: EventReservation/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,StatusId,EventTypeId,StartTime,EndTime,NumRequiredStaff,NumGuests")] EventReservation eventReservation)
        {
            if (id != eventReservation.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                eventReservation.EventType = _context.EventTypes.Where(et => et.Id == eventReservation.EventTypeId).Single();
                eventReservation.Status = _context.EventReservationStatuses.Where(s => s.Id == eventReservation.StatusId).Single();

                eventReservation.StartTime = eventReservation.StartTime.ToUniversalTime();
                eventReservation.EndTime = eventReservation.EndTime.ToUniversalTime();

                try
                {
                    _context.Update(eventReservation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventReservationExists(eventReservation.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["EventTypeId"] = new SelectList(_context.EventTypes, "Id", "Name", eventReservation.EventTypeId);
            ViewData["StatusId"] = new SelectList(_context.EventReservationStatuses, "Id", "Name", eventReservation.StatusId);
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

        private bool EventReservationExists(Guid id)
        {
            return _context.EventReservations.Any(e => e.Id == id);
        }
    }
}
