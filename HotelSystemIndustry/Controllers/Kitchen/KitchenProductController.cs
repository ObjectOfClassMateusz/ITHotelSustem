using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HotelSystemIndustry.Infrastructure;
using HotelSystemIndustry.Models.Kitchen;

namespace HotelSystemIndustry.Controllers.Kitchen
{
    public class KitchenProductController : Controller
    {
        private readonly HotelDbContext _context;

        public KitchenProductController(HotelDbContext context)
        {
            _context = context;
        }

        // GET: KitchenProduct
        public async Task<IActionResult> Index()
        {
            return View(await _context.KitchenProducts.ToListAsync());
        }

        // GET: KitchenProduct/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kitchenProduct = await _context.KitchenProducts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (kitchenProduct == null)
            {
                return NotFound();
            }

            return View(kitchenProduct);
        }

        // GET: KitchenProduct/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: KitchenProduct/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,ContainsAlcohol,Price")] KitchenProduct kitchenProduct)
        {
            if (ModelState.IsValid)
            {
                kitchenProduct.Id = Guid.NewGuid();
                _context.Add(kitchenProduct);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(kitchenProduct);
        }

        // GET: KitchenProduct/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kitchenProduct = await _context.KitchenProducts.FindAsync(id);
            if (kitchenProduct == null)
            {
                return NotFound();
            }
            return View(kitchenProduct);
        }

        // POST: KitchenProduct/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name,ContainsAlcohol,Price")] KitchenProduct kitchenProduct)
        {
            if (id != kitchenProduct.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(kitchenProduct);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KitchenProductExists(kitchenProduct.Id))
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
            return View(kitchenProduct);
        }

        // GET: KitchenProduct/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kitchenProduct = await _context.KitchenProducts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (kitchenProduct == null)
            {
                return NotFound();
            }

            return View(kitchenProduct);
        }

        // POST: KitchenProduct/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var kitchenProduct = await _context.KitchenProducts.FindAsync(id);
            if (kitchenProduct != null)
            {
                _context.KitchenProducts.Remove(kitchenProduct);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool KitchenProductExists(Guid id)
        {
            return _context.KitchenProducts.Any(e => e.Id == id);
        }
    }
}
