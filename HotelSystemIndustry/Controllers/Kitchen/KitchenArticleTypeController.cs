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
    [Authorize(Roles="KitchenEmployee,Admin")]
    public class KitchenArticleTypeController : Controller
    {
        private readonly HotelDbContext _context;

        public KitchenArticleTypeController(HotelDbContext context)
        {
            _context = context;
        }

        // GET: KitchenArticleType
        public async Task<IActionResult> Index()
        {
            return View(await _context.KitchenArticleTypes.ToListAsync());
        }

        // GET: KitchenArticleType/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kitchenArticleType = await _context.KitchenArticleTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (kitchenArticleType == null)
            {
                return NotFound();
            }

            return View(kitchenArticleType);
        }

        // GET: KitchenArticleType/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: KitchenArticleType/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Value,IsActive,Description")] KitchenArticleType kitchenArticleType)
        {
            if (ModelState.IsValid)
            {
                kitchenArticleType.Id = Guid.NewGuid();
                _context.Add(kitchenArticleType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(kitchenArticleType);
        }

        // GET: KitchenArticleType/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kitchenArticleType = await _context.KitchenArticleTypes.FindAsync(id);
            if (kitchenArticleType == null)
            {
                return NotFound();
            }
            return View(kitchenArticleType);
        }

        // POST: KitchenArticleType/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name,Value,IsActive,Description")] KitchenArticleType kitchenArticleType)
        {
            if (id != kitchenArticleType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(kitchenArticleType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KitchenArticleTypeExists(kitchenArticleType.Id))
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
            return View(kitchenArticleType);
        }

        // GET: KitchenArticleType/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kitchenArticleType = await _context.KitchenArticleTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (kitchenArticleType == null)
            {
                return NotFound();
            }

            return View(kitchenArticleType);
        }

        // POST: KitchenArticleType/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var kitchenArticleType = await _context.KitchenArticleTypes.FindAsync(id);
            if (kitchenArticleType != null)
            {
                _context.KitchenArticleTypes.Remove(kitchenArticleType);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool KitchenArticleTypeExists(Guid id)
        {
            return _context.KitchenArticleTypes.Any(e => e.Id == id);
        }
    }
}
