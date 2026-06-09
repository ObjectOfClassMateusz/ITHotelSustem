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
    public class EventHallController : Controller
    {
        private readonly HotelDbContext _context;

        public EventHallController(HotelDbContext context)
        {
            _context = context;
        }

        // GET: EventHall
        [Authorize(Roles = "Admin,HotelEmployee")]
        public async Task<IActionResult> Index()
        {
            var hotelDbContext = _context.EventHalls.Include(e => e.Hotel);
            return View(await hotelDbContext.ToListAsync());
        }

        // GET: EventHall/Details/5
        [Authorize(Roles = "Admin,HotelEmployee")]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventHall = await _context.EventHalls
                .Include(e => e.Hotel)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (eventHall == null)
            {
                return NotFound();
            }

            return View(eventHall);
        }

        // GET: EventHall/Create
        [Authorize(Roles = "Admin,HotelEmployee")]
        public IActionResult Create()
        {
            ViewData["HotelId"] = new SelectList(_context.Hotels, "Id", "Name");
            return View();
        }

        // POST: EventHall/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,HotelEmployee")]
        public async Task<IActionResult> Create([Bind("Id,Name,NumMaxGuests,HotelId,ReservationPrice")] EventHall eventHall)
        {
            if (ModelState.IsValid)
            {
                eventHall.Id = Guid.NewGuid();
                _context.Add(eventHall);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["HotelId"] = new SelectList(_context.Hotels, "Id", "Name", eventHall.HotelId);
            return View(eventHall);
        }

        // GET: EventHall/Edit/5
        [Authorize(Roles = "Admin,HotelEmployee")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventHall = await _context.EventHalls.FindAsync(id);
            if (eventHall == null)
            {
                return NotFound();
            }
            ViewData["HotelId"] = new SelectList(_context.Hotels, "Id", "Name", eventHall.HotelId);
            return View(eventHall);
        }

        // POST: EventHall/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,HotelEmployee")]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name,NumMaxGuests,HotelId,ReservationPrice")] EventHall eventHall)
        {
            if (id != eventHall.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(eventHall);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventHallExists(eventHall.Id))
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
            ViewData["HotelId"] = new SelectList(_context.Hotels, "Id", "Name", eventHall.HotelId);
            return View(eventHall);
        }

        // GET: EventHall/Delete/5
        [Authorize(Roles = "Admin,HotelEmployee")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventHall = await _context.EventHalls
                .Include(e => e.Hotel)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (eventHall == null)
            {
                return NotFound();
            }

            return View(eventHall);
        }

        // POST: EventHall/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,HotelEmployee")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var eventHall = await _context.EventHalls.FindAsync(id);
            if (eventHall != null)
            {
                _context.EventHalls.Remove(eventHall);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventHallExists(Guid id)
        {
            return _context.EventHalls.Any(e => e.Id == id);
        }
    }
}
