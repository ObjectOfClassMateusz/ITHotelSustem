using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HotelSystemIndustry.Infrastructure;
using HotelSystemIndustry.Models.Trading;

namespace HotelSystemIndustry.Controllers.Trading
{
    public class SaleItemController : Controller
    {
        private readonly HotelDbContext _context;

        public SaleItemController(HotelDbContext context)
        {
            _context = context;
        }

        // GET: SaleItem
        public async Task<IActionResult> Index()
        {
            var hotelDbContext = _context.SaleItems.Include(s => s.Type);
            return View(await hotelDbContext.ToListAsync());
        }

        // GET: SaleItem/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var saleItem = await _context.SaleItems
                .Include(s => s.Type)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (saleItem == null)
            {
                return NotFound();
            }

            return View(saleItem);
        }

        // GET: SaleItem/Create
        public IActionResult Create()
        {
            ViewData["TypeId"] = new SelectList(_context.SaleItemTypes, "Id", "Name");
            return View();
        }

        // POST: SaleItem/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,TypeId,ContainsAlcohol")] SaleItem saleItem)
        {
            if (ModelState.IsValid && saleItem.TypeId != Guid.Empty)
            {
                saleItem.Type = _context.SaleItemTypes.Where(t => t.Id == saleItem.TypeId).Single();

                saleItem.Id = Guid.NewGuid();
                _context.Add(saleItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["TypeId"] = new SelectList(_context.SaleItemTypes, "Id", "Name", saleItem.TypeId);
            return View(saleItem);
        }

        // GET: SaleItem/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var saleItem = await _context.SaleItems.FindAsync(id);
            if (saleItem == null)
            {
                return NotFound();
            }
            ViewData["TypeId"] = new SelectList(_context.SaleItemTypes, "Id", "Name", saleItem.TypeId);
            return View(saleItem);
        }

        // POST: SaleItem/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name,TypeId,ContainsAlcohol")] SaleItem saleItem)
        {
            if (id != saleItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                saleItem.Type = _context.SaleItemTypes.Where(t => t.Id == saleItem.TypeId).Single();
                
                try
                {
                    _context.Update(saleItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SaleItemExists(saleItem.Id))
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
            ViewData["TypeId"] = new SelectList(_context.SaleItemTypes, "Id", "Name", saleItem.TypeId);
            return View(saleItem);
        }

        // GET: SaleItem/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var saleItem = await _context.SaleItems
                .Include(s => s.Type)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (saleItem == null)
            {
                return NotFound();
            }

            return View(saleItem);
        }

        // POST: SaleItem/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var saleItem = await _context.SaleItems.FindAsync(id);
            if (saleItem != null)
            {
                _context.SaleItems.Remove(saleItem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SaleItemExists(Guid id)
        {
            return _context.SaleItems.Any(e => e.Id == id);
        }
    }
}
