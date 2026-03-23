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
    public class EquipmentInstanceController : Controller
    {
        private readonly HotelDbContext _context;

        public EquipmentInstanceController(HotelDbContext context)
        {
            _context = context;
        }

        // GET: EquipmentInstance
        public async Task<IActionResult> Index()
        {
            var hotelDbContext = _context.EquipmentInstances.Include(e => e.Equipment);
            return View(await hotelDbContext.ToListAsync());
        }

        // GET: EquipmentInstance/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var equipmentInstance = await _context.EquipmentInstances
                .Include(e => e.Equipment)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (equipmentInstance == null)
            {
                return NotFound();
            }

            return View(equipmentInstance);
        }

        // GET: EquipmentInstance/Create
        public IActionResult Create()
        {
            ViewData["EquipmentId"] = new SelectList(_context.Equipment, "Id", "Name");
            return View();
        }

        // POST: EquipmentInstance/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,EquipmentId,ReservationPrice")] EquipmentInstance equipmentInstance)
        {
            if (ModelState.IsValid)
            {
                equipmentInstance.Id = Guid.NewGuid();
                _context.Add(equipmentInstance);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EquipmentId"] = new SelectList(_context.Equipment, "Id", "Name", equipmentInstance.EquipmentId);
            return View(equipmentInstance);
        }

        // GET: EquipmentInstance/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var equipmentInstance = await _context.EquipmentInstances.FindAsync(id);
            if (equipmentInstance == null)
            {
                return NotFound();
            }
            ViewData["EquipmentId"] = new SelectList(_context.Equipment, "Id", "Name", equipmentInstance.EquipmentId);
            return View(equipmentInstance);
        }

        // POST: EquipmentInstance/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,EquipmentId,ReservationPrice")] EquipmentInstance equipmentInstance)
        {
            if (id != equipmentInstance.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(equipmentInstance);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EquipmentInstanceExists(equipmentInstance.Id))
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
            ViewData["EquipmentId"] = new SelectList(_context.Equipment, "Id", "Name", equipmentInstance.EquipmentId);
            return View(equipmentInstance);
        }

        // GET: EquipmentInstance/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var equipmentInstance = await _context.EquipmentInstances
                .Include(e => e.Equipment)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (equipmentInstance == null)
            {
                return NotFound();
            }

            return View(equipmentInstance);
        }

        // POST: EquipmentInstance/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var equipmentInstance = await _context.EquipmentInstances.FindAsync(id);
            if (equipmentInstance != null)
            {
                _context.EquipmentInstances.Remove(equipmentInstance);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EquipmentInstanceExists(Guid id)
        {
            return _context.EquipmentInstances.Any(e => e.Id == id);
        }
    }
}
