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
    public class OrderController : Controller
    {
        private readonly HotelDbContext _context;

        public OrderController(HotelDbContext context)
        {
            _context = context;
        }

        // GET: Order
        public async Task<IActionResult> Index()
        {
            Guid hotelId = await GetCurrentHotelId();

            var hotelDbContext = _context.KitchenOrders
                .Where(o => o.HotelId == hotelId)
                .Include(o => o.Hotel)
                .Include(o => o.Type);
            return View(await hotelDbContext.ToListAsync());
        }

        // GET: Order/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.KitchenOrders
                .Include(o => o.Hotel)
                .Include(o => o.Type)
                .Include(o => o.Products)
                    !.ThenInclude(p => p.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }


        // GET: Order/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.KitchenOrders
                .Include(o => o.Hotel)
                .Include(o => o.Type)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Order/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var order = await _context.KitchenOrders.FindAsync(id);
            if (order != null)
            {
                _context.KitchenOrders.Remove(order);
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
