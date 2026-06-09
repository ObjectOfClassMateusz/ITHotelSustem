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
    [Authorize(Roles = "Admin,TradingEmployee")]
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
            var hotelId = await GetCurrentHotelId();

            var hotelDbContext = _context.Purchases
                .Include(p => p.ShopPoint)
                .Where(p => p.ShopPoint!.HotelId == hotelId);
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

        // GET: Purchase/Delete/5
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
