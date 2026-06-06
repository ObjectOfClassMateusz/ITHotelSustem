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
    public class EventReservationStatusController : Controller
    {
        private readonly HotelDbContext _context;

        public EventReservationStatusController(HotelDbContext context)
        {
            _context = context;
        }

        // GET: EventReservationStatus
        public async Task<IActionResult> Index()
        {
            return View(await _context.EventReservationStatuses.ToListAsync());
        }

        // GET: EventReservationStatus/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventReservationStatus = await _context.EventReservationStatuses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (eventReservationStatus == null)
            {
                return NotFound();
            }

            return View(eventReservationStatus);
        }

        // GET: EventReservationStatus/Create
        [Authorize(Roles = "Admin,HotelEmployee,MaintenanceEmployee")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: EventReservationStatus/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,HotelEmployee,MaintenanceEmployee")]
        public async Task<IActionResult> Create([Bind("Id,Name,Value,IsActive,Description")] EventReservationStatus eventReservationStatus)
        {
            if (ModelState.IsValid)
            {
                eventReservationStatus.Id = Guid.NewGuid();
                _context.Add(eventReservationStatus);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(eventReservationStatus);
        }

        // GET: EventReservationStatus/Edit/5
        [Authorize(Roles = "Admin,HotelEmployee,MaintenanceEmployee")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventReservationStatus = await _context.EventReservationStatuses.FindAsync(id);
            if (eventReservationStatus == null)
            {
                return NotFound();
            }
            return View(eventReservationStatus);
        }

        // POST: EventReservationStatus/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,HotelEmployee,MaintenanceEmployee")]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name,Value,IsActive,Description")] EventReservationStatus eventReservationStatus)
        {
            if (id != eventReservationStatus.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(eventReservationStatus);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventReservationStatusExists(eventReservationStatus.Id))
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
            return View(eventReservationStatus);
        }

        // GET: EventReservationStatus/Delete/5
        [Authorize(Roles = "Admin,HotelEmployee,MaintenanceEmployee")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventReservationStatus = await _context.EventReservationStatuses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (eventReservationStatus == null)
            {
                return NotFound();
            }

            return View(eventReservationStatus);
        }

        // POST: EventReservationStatus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,HotelEmployee,MaintenanceEmployee")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var eventReservationStatus = await _context.EventReservationStatuses.FindAsync(id);
            if (eventReservationStatus != null)
            {
                _context.EventReservationStatuses.Remove(eventReservationStatus);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventReservationStatusExists(Guid id)
        {
            return _context.EventReservationStatuses.Any(e => e.Id == id);
        }
    }
}
