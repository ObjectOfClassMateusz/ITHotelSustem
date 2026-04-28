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
    [Authorize(Roles = "Admin,KitchenEmployee")]
    public class KitchenRecipeController : Controller
    {
        private readonly HotelDbContext _context;

        public KitchenRecipeController(HotelDbContext context)
        {
            _context = context;
        }

        // GET: KitchenRecipe
        public async Task<IActionResult> Index()
        {
            var hotelDbContext = _context.KitchenRecipes.Include(k => k.OutcomeProduct);
            return View(await hotelDbContext.ToListAsync());
        }

        // GET: KitchenRecipe/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kitchenRecipe = await _context.KitchenRecipes
                .Include(k => k.OutcomeProduct)
                .Include(k => k.Ingredients)
                    !.ThenInclude(kri => kri.Article)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (kitchenRecipe == null)
            {
                return NotFound();
            }

            return View(kitchenRecipe);
        }

        // GET: KitchenRecipe/Create
        public IActionResult Create()
        {
            ViewData["OutcomeProductId"] = new SelectList(_context.KitchenProducts, "Id", "Name");
            return View();
        }

        // POST: KitchenRecipe/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,OutcomeProductId,Content")] KitchenRecipe kitchenRecipe)
        {
            if (ModelState.IsValid && kitchenRecipe.OutcomeProductId != Guid.Empty)
            {
                kitchenRecipe.OutcomeProduct = _context.KitchenProducts.Where(kp => kp.Id == kitchenRecipe.OutcomeProductId).Single();

                kitchenRecipe.Id = Guid.NewGuid();
                _context.Add(kitchenRecipe);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["OutcomeProductId"] = new SelectList(_context.KitchenProducts, "Id", "Name", kitchenRecipe.OutcomeProductId);
            return View(kitchenRecipe);
        }

        // GET: KitchenRecipe/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kitchenRecipe = await _context.KitchenRecipes.FindAsync(id);
            if (kitchenRecipe == null)
            {
                return NotFound();
            }
            ViewData["OutcomeProductId"] = new SelectList(_context.KitchenProducts, "Id", "Name", kitchenRecipe.OutcomeProductId);
            return View(kitchenRecipe);
        }

        // POST: KitchenRecipe/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,OutcomeProductId,Content")] KitchenRecipe kitchenRecipe)
        {
            if (id != kitchenRecipe.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                kitchenRecipe.OutcomeProduct = _context.KitchenProducts.Where(kp => kp.Id == kitchenRecipe.OutcomeProductId).Single();
                
                try
                {
                    _context.Update(kitchenRecipe);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KitchenRecipeExists(kitchenRecipe.Id))
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
            ViewData["OutcomeProductId"] = new SelectList(_context.KitchenProducts, "Id", "Name", kitchenRecipe.OutcomeProductId);
            return View(kitchenRecipe);
        }

        // GET: KitchenRecipe/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kitchenRecipe = await _context.KitchenRecipes
                .Include(k => k.OutcomeProduct)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (kitchenRecipe == null)
            {
                return NotFound();
            }

            return View(kitchenRecipe);
        }

        // POST: KitchenRecipe/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var kitchenRecipe = await _context.KitchenRecipes.FindAsync(id);
            if (kitchenRecipe != null)
            {
                _context.KitchenRecipes.Remove(kitchenRecipe);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool KitchenRecipeExists(Guid id)
        {
            return _context.KitchenRecipes.Any(e => e.Id == id);
        }
    }
}
