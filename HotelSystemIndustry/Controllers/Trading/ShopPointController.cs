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
    public class ShopPointController : Controller
    {
        private readonly HotelDbContext _context;

        public ShopPointController(HotelDbContext context)
        {
            _context = context;
        }

        // GET: ShopPoint
        public async Task<IActionResult> Index()
        {
            var hotelId = await GetCurrentHotelId();

            var hotelDbContext = _context.ShopPoints
                .Where(s => s.HotelId == hotelId)
                .Include(s => s.Hotel);
            return View(await hotelDbContext.ToListAsync());
        }

        // GET: ShopPoint/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shopPoint = await _context.ShopPoints
                .Include(s => s.Hotel)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (shopPoint == null)
            {
                return NotFound();
            }

            return View(shopPoint);
        }

        // GET: ShopPoint/Create
        [Authorize(Roles = "Admin,TradingEmployee,MaintainanceEmployee")]
        public IActionResult Create()
        {
            ViewData["HotelId"] = new SelectList(_context.Hotels, "Id", "Name");
            return View();
        }

        // POST: ShopPoint/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,TradingEmployee,MaintainanceEmployee")]
        public async Task<IActionResult> Create([Bind("Id,Location,HotelId")] ShopPoint shopPoint)
        {
            if (ModelState.IsValid)
            {
                shopPoint.Id = Guid.NewGuid();
                _context.Add(shopPoint);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["HotelId"] = new SelectList(_context.Hotels, "Id", "Name", shopPoint.HotelId);
            return View(shopPoint);
        }

        // GET: ShopPoint/Edit/5
        [Authorize(Roles = "Admin,TradingEmployee,MaintainanceEmployee")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shopPoint = await _context.ShopPoints.FindAsync(id);
            if (shopPoint == null)
            {
                return NotFound();
            }
            ViewData["HotelId"] = new SelectList(_context.Hotels, "Id", "Name", shopPoint.HotelId);
            return View(shopPoint);
        }

        // POST: ShopPoint/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,TradingEmployee,MaintainanceEmployee")]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Location,HotelId")] ShopPoint shopPoint)
        {
            if (id != shopPoint.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(shopPoint);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShopPointExists(shopPoint.Id))
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
            ViewData["HotelId"] = new SelectList(_context.Hotels, "Id", "Name", shopPoint.HotelId);
            return View(shopPoint);
        }

        // GET: ShopPoint/Delete/5
        [Authorize(Roles = "Admin,TradingEmployee,MaintainanceEmployee")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shopPoint = await _context.ShopPoints
                .Include(s => s.Hotel)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (shopPoint == null)
            {
                return NotFound();
            }

            return View(shopPoint);
        }

        // POST: ShopPoint/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,TradingEmployee,MaintainanceEmployee")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var shopPoint = await _context.ShopPoints.FindAsync(id);
            if (shopPoint != null)
            {
                _context.ShopPoints.Remove(shopPoint);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ShopPointExists(Guid id)
        {
            return _context.ShopPoints.Any(e => e.Id == id);
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
