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
            var hotelDbContext = _context.KitchenOrders.Include(o => o.Hotel).Include(o => o.Type);
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

        // GET: Order/Create
        public IActionResult Create()
        {
            ViewData["HotelId"] = new SelectList(_context.Hotels, "Id", "Name");
            ViewData["TypeId"] = new SelectList(_context.KitchenOrderTypes, "Id", "Name");
            return View();
        }

        // POST: Order/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SubmissionTime,RealisedTime,TypeId,HotelId,DeliveryDestination")] Order order)
        {
            if (ModelState.IsValid && order.TypeId != Guid.Empty)
            {
                order.Type = _context.KitchenOrderTypes.Where(t => t.Id == order.TypeId).Single();

                order.SubmissionTime = order.SubmissionTime.ToUniversalTime();
                order.RealisedTime = order.RealisedTime?.ToUniversalTime();

                order.Id = Guid.NewGuid();
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["HotelId"] = new SelectList(_context.Hotels, "Id", "Name", order.HotelId);
            ViewData["TypeId"] = new SelectList(_context.KitchenOrderTypes, "Id", "Name", order.TypeId);
            return View(order);
        }

        // GET: Order/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.KitchenOrders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["HotelId"] = new SelectList(_context.Hotels, "Id", "Name", order.HotelId);
            ViewData["TypeId"] = new SelectList(_context.KitchenOrderTypes, "Id", "Name", order.TypeId);
            return View(order);
        }

        // POST: Order/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,SubmissionTime,RealisedTime,TypeId,HotelId,DeliveryDestination")] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                order.Type = _context.KitchenOrderTypes.Where(t => t.Id == order.TypeId).Single();

                order.SubmissionTime = order.SubmissionTime.ToUniversalTime();
                order.RealisedTime = order.RealisedTime?.ToUniversalTime();

                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
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
            ViewData["HotelId"] = new SelectList(_context.Hotels, "Id", "Name", order.HotelId);
            ViewData["TypeId"] = new SelectList(_context.KitchenOrderTypes, "Id", "Name", order.TypeId);
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

        private bool OrderExists(Guid id)
        {
            return _context.KitchenOrders.Any(e => e.Id == id);
        }
    }
}
