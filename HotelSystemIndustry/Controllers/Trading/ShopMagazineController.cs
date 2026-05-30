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
    public class ShopMagazineController : Controller
    {
        private readonly HotelDbContext _context;

        public ShopMagazineController(HotelDbContext context)
        {
            _context = context;
        }

        // GET: ShopMagazine
        public async Task<IActionResult> Index()
        {
            var hotelDbContext = _context.ShopMagazines.Include(s => s.Hotel);
            return View(await hotelDbContext.ToListAsync());
        }

        // GET: ShopMagazine/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shopMagazine = await _context.ShopMagazines
                .Include(s => s.Hotel)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (shopMagazine == null)
            {
                return NotFound();
            }

            return View(shopMagazine);
        }

        // GET: ShopMagazine/Create
        [Authorize(Roles = "Admin,TradingEmployee,MaintainanceEmployee")]
        public IActionResult Create()
        {
            ViewData["HotelId"] = new SelectList(_context.Hotels, "Id", "Description");
            return View();
        }

        // POST: ShopMagazine/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,TradingEmployee,MaintainanceEmployee")]
        public async Task<IActionResult> Create([Bind("Id,Location,HotelId")] ShopMagazine shopMagazine)
        {
            if (ModelState.IsValid)
            {
                shopMagazine.Id = Guid.NewGuid();
                _context.Add(shopMagazine);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["HotelId"] = new SelectList(_context.Hotels, "Id", "Name", shopMagazine.HotelId);
            return View(shopMagazine);
        }

        // GET: ShopMagazine/Edit/5
        [Authorize(Roles = "Admin,TradingEmployee,MaintainanceEmployee")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shopMagazine = await _context.ShopMagazines.FindAsync(id);
            if (shopMagazine == null)
            {
                return NotFound();
            }
            ViewData["HotelId"] = new SelectList(_context.Hotels, "Id", "Name", shopMagazine.HotelId);
            return View(shopMagazine);
        }

        // POST: ShopMagazine/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,TradingEmployee,MaintainanceEmployee")]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Location,HotelId")] ShopMagazine shopMagazine)
        {
            if (id != shopMagazine.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(shopMagazine);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShopMagazineExists(shopMagazine.Id))
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
            ViewData["HotelId"] = new SelectList(_context.Hotels, "Id", "Name", shopMagazine.HotelId);
            return View(shopMagazine);
        }

        // GET: ShopMagazine/Delete/5
        [Authorize(Roles = "Admin,TradingEmployee,MaintainanceEmployee")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shopMagazine = await _context.ShopMagazines
                .Include(s => s.Hotel)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (shopMagazine == null)
            {
                return NotFound();
            }

            return View(shopMagazine);
        }

        // POST: ShopMagazine/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,TradingEmployee,MaintainanceEmployee")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var shopMagazine = await _context.ShopMagazines.FindAsync(id);
            if (shopMagazine != null)
            {
                _context.ShopMagazines.Remove(shopMagazine);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ShopMagazineExists(Guid id)
        {
            return _context.ShopMagazines.Any(e => e.Id == id);
        }
    }
}
