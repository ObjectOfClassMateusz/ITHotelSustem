using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HotelSystemIndustry.Infrastructure;
using HotelSystemIndustry.Models.Trading;
using Microsoft.AspNetCore.Authorization;

namespace HotelSystemIndustry.Controllers.Trading
{
    public class PurchaseController : Controller
    {
        private readonly HotelDbContext _context;

        public PurchaseController(HotelDbContext context)
        {
            _context = context;
        }


        // GET: Purchase
        public async Task<IActionResult> Index()
        {
            var hotelDbContext = _context.Purchases.Include(p => p.ShopPoint);
            return View(await hotelDbContext.ToListAsync());
        }

        // GET: Purchase/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchase = await _context.Purchases
                .Include(p => p.ShopPoint)
                .Include(p => p.Items)
                    !.ThenInclude(p => p.SaleItem)
                        .ThenInclude(s => s!.Type)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (purchase == null)
            {
                return NotFound();
            }

            return View(purchase);
        }

        // GET: Purchase/Create
        [Authorize(Roles = "Admin,TradingEmployee")]
        public IActionResult Create()
        {
            ViewData["ShopPointId"] = new SelectList(_context.ShopPoints, "Id", "Location");
            return View();
        }

        // POST: Purchase/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,TradingEmployee")]
        public async Task<IActionResult> Create([Bind("Id,TransactionDate,ShopPointId")] Purchase purchase)
        {
            if (ModelState.IsValid)
            {
                purchase.ShopPoint = _context.ShopPoints.Where(sp => sp.Id == purchase.ShopPointId).Single();
                
                purchase.TransactionDate = purchase.TransactionDate.ToUniversalTime();

                purchase.Id = Guid.NewGuid();
                _context.Add(purchase);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ShopPointId"] = new SelectList(_context.ShopPoints, "Id", "Location");
            return View(purchase);
        }

        // GET: Purchase/Edit/5
        [Authorize(Roles = "Admin,TradingEmployee")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchase = await _context.Purchases.FindAsync(id);
            if (purchase == null)
            {
                return NotFound();
            }
            ViewData["ShopPointId"] = new SelectList(_context.ShopPoints, "Id", "Location", purchase.ShopPointId);
            return View(purchase);
        }

        // POST: Purchase/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,TradingEmployee")]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,TransactionDate,ShopPointId")] Purchase purchase)
        {
            if (id != purchase.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                purchase.ShopPoint = _context.ShopPoints.Where(sp => sp.Id == purchase.ShopPointId).Single();
                
                purchase.TransactionDate = purchase.TransactionDate.ToUniversalTime();

                try
                {
                    _context.Update(purchase);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PurchaseExists(purchase.Id))
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
            ViewData["ShopPointId"] = new SelectList(_context.ShopPoints, "Id", "Location", purchase.ShopPointId);
            return View(purchase);
        }

        // GET: Purchase/Delete/5
        [Authorize(Roles = "Admin,TradingEmployee")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchase = await _context.Purchases
                .Include(p => p.ShopPoint)
                .Include(p => p.Items)
                    !.ThenInclude(p => p.SaleItem)
                        .ThenInclude(s => s!.Type)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (purchase == null)
            {
                return NotFound();
            }

            return View(purchase);
        }

        // POST: Purchase/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,TradingEmployee")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var purchase = await _context.Purchases.FindAsync(id);
            if (purchase != null)
            {
                _context.Purchases.Remove(purchase);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PurchaseExists(Guid id)
        {
            return _context.Purchases.Any(e => e.Id == id);
        }
    }
}
