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
    public class SaleItemInstanceController : Controller
    {
        private readonly HotelDbContext _context;

        public SaleItemInstanceController(HotelDbContext context)
        {
            _context = context;
        }

        // GET: SaleItemInstance
        public async Task<IActionResult> Index()
        {
            var hotelDbContext = _context.SaleItemInstances.Include(s => s.Item).Include(s => s.Magazine);
            return View(await hotelDbContext.ToListAsync());
        }

        // GET: SaleItemInstance/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var saleItemInstance = await _context.SaleItemInstances
                .Include(s => s.Item)
                .Include(s => s.Magazine)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (saleItemInstance == null)
            {
                return NotFound();
            }

            return View(saleItemInstance);
        }

        // GET: SaleItemInstance/Create
        public IActionResult Create()
        {
            ViewData["ItemId"] = new SelectList(_context.SaleItems, "Id", "Name");
            ViewData["MagazineId"] = new SelectList(_context.ShopMagazines, "Id", "Location");
            return View();
        }

        // POST: SaleItemInstance/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ItemId,MagazineId,Variant,Count,ExpireDate")] SaleItemInstance saleItemInstance)
        {
            if (ModelState.IsValid)
            {
                saleItemInstance.Item = _context.SaleItems.Where(si => si.Id == saleItemInstance.ItemId).Single();
                saleItemInstance.Magazine = _context.ShopMagazines.Where(sm => sm.Id == saleItemInstance.MagazineId).Single();

                saleItemInstance.ExpireDate = saleItemInstance.ExpireDate?.ToUniversalTime();

                saleItemInstance.Id = Guid.NewGuid();
                _context.Add(saleItemInstance);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ItemId"] = new SelectList(_context.SaleItems, "Id", "Name", saleItemInstance.ItemId);
            ViewData["MagazineId"] = new SelectList(_context.ShopMagazines, "Id", "Location", saleItemInstance.MagazineId);
            return View(saleItemInstance);
        }

        // GET: SaleItemInstance/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var saleItemInstance = await _context.SaleItemInstances.FindAsync(id);
            if (saleItemInstance == null)
            {
                return NotFound();
            }
            ViewData["ItemId"] = new SelectList(_context.SaleItems, "Id", "Name", saleItemInstance.ItemId);
            ViewData["MagazineId"] = new SelectList(_context.ShopMagazines, "Id", "Location", saleItemInstance.MagazineId);
            return View(saleItemInstance);
        }

        // POST: SaleItemInstance/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,ItemId,MagazineId,Variant,Count,ExpireDate")] SaleItemInstance saleItemInstance)
        {
            if (id != saleItemInstance.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                saleItemInstance.Item = _context.SaleItems.Where(si => si.Id == saleItemInstance.ItemId).Single();
                saleItemInstance.Magazine = _context.ShopMagazines.Where(sm => sm.Id == saleItemInstance.MagazineId).Single();

                saleItemInstance.ExpireDate = saleItemInstance.ExpireDate?.ToUniversalTime();

                try
                {
                    _context.Update(saleItemInstance);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SaleItemInstanceExists(saleItemInstance.Id))
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
            ViewData["ItemId"] = new SelectList(_context.SaleItems, "Id", "Name", saleItemInstance.ItemId);
            ViewData["MagazineId"] = new SelectList(_context.ShopMagazines, "Id", "Location", saleItemInstance.MagazineId);
            return View(saleItemInstance);
        }

        // GET: SaleItemInstance/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var saleItemInstance = await _context.SaleItemInstances
                .Include(s => s.Item)
                .Include(s => s.Magazine)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (saleItemInstance == null)
            {
                return NotFound();
            }

            return View(saleItemInstance);
        }

        // POST: SaleItemInstance/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var saleItemInstance = await _context.SaleItemInstances.FindAsync(id);
            if (saleItemInstance != null)
            {
                _context.SaleItemInstances.Remove(saleItemInstance);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SaleItemInstanceExists(Guid id)
        {
            return _context.SaleItemInstances.Any(e => e.Id == id);
        }
    }
}
