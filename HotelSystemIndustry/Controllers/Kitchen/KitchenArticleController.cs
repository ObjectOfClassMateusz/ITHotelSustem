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
    public class KitchenArticleController : Controller
    {
        private readonly HotelDbContext _context;

        public KitchenArticleController(HotelDbContext context)
        {
            _context = context;
        }

        // GET: KitchenArticle
        public async Task<IActionResult> Index()
        {
            var hotelDbContext = _context.KitchenArticles.Include(k => k.Type);
            return View(await hotelDbContext.ToListAsync());
        }

        // GET: KitchenArticle/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kitchenArticle = await _context.KitchenArticles
                .Include(k => k.Type)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (kitchenArticle == null)
            {
                return NotFound();
            }

            return View(kitchenArticle);
        }

        // GET: KitchenArticle/Create
        [Authorize(Roles = "Admin,KitchenEmployee,MaintainanceEmployee")]
        public IActionResult Create()
        {
            ViewData["TypeId"] = new SelectList(_context.KitchenArticleTypes, "Id", "Name");
            return View();
        }

        // POST: KitchenArticle/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,KitchenEmployee,MaintainanceEmployee")]
        public async Task<IActionResult> Create([Bind("Id,Name,TypeId")] KitchenArticle kitchenArticle)
        {
            if (ModelState.IsValid && kitchenArticle.TypeId != Guid.Empty)
            {
                kitchenArticle.Type = _context.KitchenArticleTypes.Where(ka => ka.Id == kitchenArticle.TypeId).Single();

                kitchenArticle.Id = Guid.NewGuid();
                _context.Add(kitchenArticle);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["TypeId"] = new SelectList(_context.KitchenArticleTypes, "Id", "Name", kitchenArticle.TypeId);
            return View(kitchenArticle);
        }

        // GET: KitchenArticle/Edit/5
        [Authorize(Roles = "Admin,KitchenEmployee,MaintainanceEmployee")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kitchenArticle = await _context.KitchenArticles.FindAsync(id);
            if (kitchenArticle == null)
            {
                return NotFound();
            }
            ViewData["TypeId"] = new SelectList(_context.KitchenArticleTypes, "Id", "Name", kitchenArticle.TypeId);
            return View(kitchenArticle);
        }

        // POST: KitchenArticle/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,KitchenEmployee,MaintainanceEmployee")]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name,TypeId")] KitchenArticle kitchenArticle)
        {
            if (id != kitchenArticle.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                kitchenArticle.Type = _context.KitchenArticleTypes.Where(ka => ka.Id == kitchenArticle.TypeId).Single();
                
                try
                {
                    _context.Update(kitchenArticle);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KitchenArticleExists(kitchenArticle.Id))
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
            ViewData["TypeId"] = new SelectList(_context.KitchenArticleTypes, "Id", "Name", kitchenArticle.TypeId);
            return View(kitchenArticle);
        }

        // GET: KitchenArticle/Delete/5
        [Authorize(Roles = "Admin,KitchenEmployee,MaintainanceEmployee")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kitchenArticle = await _context.KitchenArticles
                .Include(k => k.Type)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (kitchenArticle == null)
            {
                return NotFound();
            }

            return View(kitchenArticle);
        }

        // POST: KitchenArticle/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,KitchenEmployee,MaintainanceEmployee")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var kitchenArticle = await _context.KitchenArticles.FindAsync(id);
            if (kitchenArticle != null)
            {
                _context.KitchenArticles.Remove(kitchenArticle);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool KitchenArticleExists(Guid id)
        {
            return _context.KitchenArticles.Any(e => e.Id == id);
        }
    }
}
