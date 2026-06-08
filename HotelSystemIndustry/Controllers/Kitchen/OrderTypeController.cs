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
    public class OrderTypeController : Controller
    {
        private readonly HotelDbContext _context;

        public OrderTypeController(HotelDbContext context)
        {
            _context = context;
        }

        // GET: OrderType
        [Authorize(Roles="KitchenEmployee,Admin")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.KitchenOrderTypes.ToListAsync());
        }

        // GET: OrderType/Details/5
        [Authorize(Roles="KitchenEmployee,Admin")]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderType = await _context.KitchenOrderTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (orderType == null)
            {
                return NotFound();
            }

            return View(orderType);
        }

        // GET: OrderType/Create
        [Authorize(Roles = "Admin,KitchenEmployee")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: OrderType/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,KitchenEmployee")]
        public async Task<IActionResult> Create([Bind("Id,Name,Value,IsActive,Description")] OrderType orderType)
        {
            if (ModelState.IsValid)
            {
                orderType.Id = Guid.NewGuid();
                _context.Add(orderType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(orderType);
        }

        // GET: OrderType/Edit/5
        [Authorize(Roles = "Admin,KitchenEmployee")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderType = await _context.KitchenOrderTypes.FindAsync(id);
            if (orderType == null)
            {
                return NotFound();
            }
            return View(orderType);
        }

        // POST: OrderType/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,KitchenEmployee")]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name,Value,IsActive,Description")] OrderType orderType)
        {
            if (id != orderType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orderType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderTypeExists(orderType.Id))
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
            return View(orderType);
        }

        // GET: OrderType/Delete/5
        [Authorize(Roles = "Admin,KitchenEmployee")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderType = await _context.KitchenOrderTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (orderType == null)
            {
                return NotFound();
            }

            return View(orderType);
        }

        // POST: OrderType/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,KitchenEmployee")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var orderType = await _context.KitchenOrderTypes.FindAsync(id);
            if (orderType != null)
            {
                _context.KitchenOrderTypes.Remove(orderType);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderTypeExists(Guid id)
        {
            return _context.KitchenOrderTypes.Any(e => e.Id == id);
        }
    }
}
