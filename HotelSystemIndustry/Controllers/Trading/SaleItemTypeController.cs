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
    public class SaleItemTypeController : Controller
    {
        private readonly HotelDbContext _context;

        public SaleItemTypeController(HotelDbContext context)
        {
            _context = context;
        }

        // GET: SaleItemType
        public async Task<IActionResult> Index()
        {
            return View(await _context.SaleItemTypes.ToListAsync());
        }

        // GET: SaleItemType/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var saleItemType = await _context.SaleItemTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (saleItemType == null)
            {
                return NotFound();
            }

            return View(saleItemType);
        }

        // GET: SaleItemType/Create
        [Authorize(Roles = "Admin,TradingEmployee,MaintainanceEmployee")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: SaleItemType/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,TradingEmployee,MaintainanceEmployee")]
        public async Task<IActionResult> Create([Bind("Id,Name,Value,IsActive,Description")] SaleItemType saleItemType)
        {
            if (ModelState.IsValid)
            {
                saleItemType.Id = Guid.NewGuid();
                _context.Add(saleItemType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(saleItemType);
        }

        // GET: SaleItemType/Edit/5
        [Authorize(Roles = "Admin,TradingEmployee,MaintainanceEmployee")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var saleItemType = await _context.SaleItemTypes.FindAsync(id);
            if (saleItemType == null)
            {
                return NotFound();
            }
            return View(saleItemType);
        }

        // POST: SaleItemType/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,TradingEmployee,MaintainanceEmployee")]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name,Value,IsActive,Description")] SaleItemType saleItemType)
        {
            if (id != saleItemType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(saleItemType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SaleItemTypeExists(saleItemType.Id))
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
            return View(saleItemType);
        }

        // GET: SaleItemType/Delete/5
        [Authorize(Roles = "Admin,TradingEmployee,MaintainanceEmployee")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var saleItemType = await _context.SaleItemTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (saleItemType == null)
            {
                return NotFound();
            }

            return View(saleItemType);
        }

        // POST: SaleItemType/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,TradingEmployee,MaintainanceEmployee")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var saleItemType = await _context.SaleItemTypes.FindAsync(id);
            if (saleItemType != null)
            {
                _context.SaleItemTypes.Remove(saleItemType);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SaleItemTypeExists(Guid id)
        {
            return _context.SaleItemTypes.Any(e => e.Id == id);
        }
    }
}
