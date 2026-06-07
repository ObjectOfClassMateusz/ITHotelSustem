using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HotelSystemIndustry.Infrastructure;
using HotelSystemIndustry.Models.Kitchen;
using Microsoft.AspNetCore.Authorization;

namespace HotelSystemIndustry.Controllers.Kitchen
{
    [Authorize(Roles="KitchenEmployee,MaintainanceEmployee,Admin")]
    public class StorageController : Controller
    {
        private readonly HotelDbContext _context;

        public StorageController(HotelDbContext context)
        {
            _context = context;
        }

        // GET: Storage
        public async Task<IActionResult> Index()
        {
            Guid hotelId = await GetCurrentHotelId();

            var hotelDbContext = _context.KitchenStorages
                .Where(s => s.HotelId == hotelId)
                .Include(s => s.Hotel);
            return View(await hotelDbContext.ToListAsync());
        }

        // GET: Storage/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var storage = await _context.KitchenStorages
                .Include(s => s.Hotel)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (storage == null)
            {
                return NotFound();
            }

            return View(storage);
        }

        // GET: Storage/Create
        public IActionResult Create()
        {
            ViewData["HotelId"] = new SelectList(_context.Hotels, "Id", "Name");
            return View();
        }

        // POST: Storage/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Location,HotelId")] Storage storage)
        {
            if (ModelState.IsValid)
            {
                storage.Id = Guid.NewGuid();
                _context.Add(storage);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["HotelId"] = new SelectList(_context.Hotels, "Id", "Name", storage.HotelId);
            return View(storage);
        }

        // GET: Storage/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var storage = await _context.KitchenStorages.FindAsync(id);
            if (storage == null)
            {
                return NotFound();
            }
            ViewData["HotelId"] = new SelectList(_context.Hotels, "Id", "Name", storage.HotelId);
            return View(storage);
        }

        // POST: Storage/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name,Location,HotelId")] Storage storage)
        {
            if (id != storage.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(storage);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StorageExists(storage.Id))
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
            ViewData["HotelId"] = new SelectList(_context.Hotels, "Id", "Name", storage.HotelId);
            return View(storage);
        }

        // GET: Storage/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var storage = await _context.KitchenStorages
                .Include(s => s.Hotel)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (storage == null)
            {
                return NotFound();
            }

            return View(storage);
        }

        // POST: Storage/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var storage = await _context.KitchenStorages.FindAsync(id);
            if (storage != null)
            {
                _context.KitchenStorages.Remove(storage);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StorageExists(Guid id)
        {
            return _context.KitchenStorages.Any(e => e.Id == id);
        }


        private async Task<Guid> GetCurrentHotelId()
        {
            HotelChangeController hotelChangeController = new HotelChangeController(_context)
            {
                ControllerContext = this.ControllerContext
            };
            return await hotelChangeController.GetCurrentHotel();
        }
    }
}
