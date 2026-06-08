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
using HotelSystemIndustry.ViewModels.Kitchen;
using System.Collections.ObjectModel;

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
        public async Task<IActionResult> Create()
        {
            ViewData["OutcomeProductId"] = new SelectList(_context.KitchenProducts, "Id", "Name");
            ViewBag.ArticleList = new SelectList(_context.KitchenArticles, "Id", "Name");
            ViewBag.Articles = await _context.KitchenArticles.AsNoTracking().ToListAsync();
            return View(new KitchenRecipeEditViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAddIngredient(KitchenRecipeEditViewModel model)
        {
            if (model.NewIngredient.ArticleId != Guid.Empty && model.NewIngredient.Count > 0.0m)
            {
                var ing = model.Ingredients.FirstOrDefault(i => i.ArticleId == model.NewIngredient.ArticleId);

                if (ing != null)
                {
                    ing.Count += model.NewIngredient.Count;
                }
                else
                {
                    model.Ingredients.Add(model.NewIngredient);
                }

                model.NewIngredient = new KitchenIngredientEditViewModel();
            }

            ModelState.Clear();

            ViewData["OutcomeProductId"] = new SelectList(_context.KitchenProducts, "Id", "Name", model.OutcomeProductId);
            ViewBag.ArticleList = new SelectList(_context.KitchenArticles, "Id", "Name");
            ViewBag.Articles = await _context.KitchenArticles.AsNoTracking().ToListAsync();
            return View("Create", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRemoveIngredient(KitchenRecipeEditViewModel model, int index)
        {
            if (index >= 0 && index < model.Ingredients.Count)
            {
                model.Ingredients.RemoveAt(index);
            }

            ModelState.Clear();

            ViewData["OutcomeProductId"] = new SelectList(_context.KitchenProducts, "Id", "Name", model.OutcomeProductId);
            ViewBag.ArticleList = new SelectList(_context.KitchenArticles, "Id", "Name");
            ViewBag.Articles = await _context.KitchenArticles.AsNoTracking().ToListAsync();
            return View("Create", model);
        }

        // POST: KitchenRecipe/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(KitchenRecipeEditViewModel model)
        {
            if (ModelState.IsValid && model.OutcomeProductId != Guid.Empty)
            {
                KitchenRecipe kitchenRecipe = new KitchenRecipe
                {
                    Id = Guid.NewGuid(),
                    OutcomeProductId = model.OutcomeProductId,
                    OutcomeProduct = _context.KitchenProducts.FirstOrDefault(kp => kp.Id == model.OutcomeProductId),
                    Content = model.Content,
                    Ingredients = new Collection<KitchenRecipeIngredient>()
                };

                foreach (var ing in model.Ingredients)
                {
                    KitchenRecipeIngredient recipeIng = new KitchenRecipeIngredient
                    {
                        RecipeId = kitchenRecipe.Id,
                        Recipe = kitchenRecipe,
                        ArticleId = ing.ArticleId,
                        Article = _context.KitchenArticles.FirstOrDefault(ka => ka.Id == ing.ArticleId),
                        Count = ing.Count
                    };
                    kitchenRecipe.Ingredients.Add(recipeIng);
                    _context.Add(recipeIng);
                }

                _context.Add(kitchenRecipe);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["OutcomeProductId"] = new SelectList(_context.KitchenProducts, "Id", "Name", model.OutcomeProductId);
            ViewBag.ArticleList = new SelectList(_context.KitchenArticles, "Id", "Name");
            ViewBag.Articles = await _context.KitchenArticles.AsNoTracking().ToListAsync();
            return View(model);
        }

        // GET: KitchenRecipe/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kitchenRecipe = await _context.KitchenRecipes
                .Include(kr => kr.Ingredients)
                .FirstOrDefaultAsync(kr => kr.Id == id);
            if (kitchenRecipe == null)
            {
                return NotFound();
            }

            KitchenRecipeEditViewModel model = new KitchenRecipeEditViewModel
            {
                TargetRecipeId = kitchenRecipe.Id,
                OutcomeProductId = kitchenRecipe.OutcomeProductId,
                Content = kitchenRecipe.Content
            };

            foreach (var ing in kitchenRecipe.Ingredients!)
            {
                model.Ingredients.Add(new KitchenIngredientEditViewModel
                {
                    ArticleId = ing.ArticleId,
                    Count = ing.Count
                });
            }

            ViewData["OutcomeProductId"] = new SelectList(_context.KitchenProducts, "Id", "Name", kitchenRecipe.OutcomeProductId);
            ViewBag.ArticleList = new SelectList(_context.KitchenArticles, "Id", "Name");
            ViewBag.Articles = await _context.KitchenArticles.AsNoTracking().ToListAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAddIngredient(KitchenRecipeEditViewModel model)
        {
            if (model.NewIngredient.ArticleId != Guid.Empty && model.NewIngredient.Count > 0.0m)
            {
                var ing = model.Ingredients.FirstOrDefault(i => i.ArticleId == model.NewIngredient.ArticleId);

                if (ing != null)
                {
                    ing.Count += model.NewIngredient.Count;
                }
                else
                {
                    model.Ingredients.Add(model.NewIngredient);
                }

                model.NewIngredient = new KitchenIngredientEditViewModel();
            }

            ModelState.Clear();

            ViewData["OutcomeProductId"] = new SelectList(_context.KitchenProducts, "Id", "Name", model.OutcomeProductId);
            ViewBag.ArticleList = new SelectList(_context.KitchenArticles, "Id", "Name");
            ViewBag.Articles = await _context.KitchenArticles.AsNoTracking().ToListAsync();
            return View("Edit", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRemoveIngredient(KitchenRecipeEditViewModel model, int index)
        {
            if (index >= 0 && index < model.Ingredients.Count)
            {
                model.Ingredients.RemoveAt(index);
            }

            ModelState.Clear();

            ViewData["OutcomeProductId"] = new SelectList(_context.KitchenProducts, "Id", "Name", model.OutcomeProductId);
            ViewBag.ArticleList = new SelectList(_context.KitchenArticles, "Id", "Name");
            ViewBag.Articles = await _context.KitchenArticles.AsNoTracking().ToListAsync();
            return View("Edit", model);
        }

        // POST: KitchenRecipe/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(KitchenRecipeEditViewModel model)
        {
            var kitchenRecipe = await _context.KitchenRecipes
                .Include(kr => kr.Ingredients)
                .FirstOrDefaultAsync(kr => kr.Id == model.TargetRecipeId);

            if (kitchenRecipe == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                kitchenRecipe.OutcomeProductId = model.OutcomeProductId;
                kitchenRecipe.OutcomeProduct = _context.KitchenProducts.FirstOrDefault(kp => kp.Id == model.OutcomeProductId);
                kitchenRecipe.Content = model.Content;


                foreach (var ing in kitchenRecipe.Ingredients!)
                {
                    _context.KitchenRecipeIngredients.Remove(ing);
                }
                kitchenRecipe.Ingredients.Clear();

                foreach (var ing in model.Ingredients)
                {
                    KitchenRecipeIngredient recipeIng = new KitchenRecipeIngredient
                    {
                        RecipeId = kitchenRecipe.Id,
                        Recipe = kitchenRecipe,
                        ArticleId = ing.ArticleId,
                        Article = _context.KitchenArticles.FirstOrDefault(ka => ka.Id == ing.ArticleId),
                        Count = ing.Count
                    };
                    kitchenRecipe.Ingredients.Add(recipeIng);
                    _context.Add(recipeIng);
                }


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
            ViewData["OutcomeProductId"] = new SelectList(_context.KitchenProducts, "Id", "Name", model.OutcomeProductId);
            ViewBag.ArticleList = new SelectList(_context.KitchenArticles, "Id", "Name");
            ViewBag.Articles = await _context.KitchenArticles.AsNoTracking().ToListAsync();
            return View(model);
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
