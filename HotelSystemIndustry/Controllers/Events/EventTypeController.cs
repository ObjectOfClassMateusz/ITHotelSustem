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
    public class EventTypeController : Controller
    {
        private readonly HotelDbContext _context;

        public EventTypeController(HotelDbContext context)
        {
            _context = context;
        }

        // GET: EventType
        public async Task<IActionResult> Index()
        {
            return View(await _context.EventTypes.ToListAsync());
        }

        // GET: EventType/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventType = await _context.EventTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (eventType == null)
            {
                return NotFound();
            }

            return View(eventType);
        }

        // GET: EventType/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: EventType/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Value,IsActive,Description")] EventType eventType)
        {
            if (ModelState.IsValid)
            {
                eventType.Id = Guid.NewGuid();
                _context.Add(eventType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(eventType);
        }

        // GET: EventType/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventType = await _context.EventTypes.FindAsync(id);
            if (eventType == null)
            {
                return NotFound();
            }
            return View(eventType);
        }

        // POST: EventType/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name,Value,IsActive,Description")] EventType eventType)
        {
            if (id != eventType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(eventType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventTypeExists(eventType.Id))
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
            return View(eventType);
        }

        // GET: EventType/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventType = await _context.EventTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (eventType == null)
            {
                return NotFound();
            }

            return View(eventType);
        }

        // POST: EventType/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var eventType = await _context.EventTypes.FindAsync(id);
            if (eventType != null)
            {
                _context.EventTypes.Remove(eventType);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventTypeExists(Guid id)
        {
            return _context.EventTypes.Any(e => e.Id == id);
        }
    }
}
