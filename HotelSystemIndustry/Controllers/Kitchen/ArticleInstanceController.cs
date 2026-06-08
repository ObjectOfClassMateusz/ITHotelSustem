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
    [Authorize(Roles="KitchenEmployee,MaintenanceEmployee,Admin")]
    public class ArticleInstanceController : Controller
    {
        private readonly HotelDbContext _context;

        public ArticleInstanceController(HotelDbContext context)
        {
            _context = context;
        }

        // GET: ArticleInstance
        public async Task<IActionResult> Index()
        {
            Guid hotelId = await GetCurrentHotelId();

            var hotelDbContext = _context.KitchenArticleInstances
                .Include(a => a.Article)
                .Include(a => a.Storage)
                .Where(a => a.Storage!.HotelId == hotelId);
            return View(await hotelDbContext.ToListAsync());
        }

        // GET: ArticleInstance/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var articleInstance = await _context.KitchenArticleInstances
                .Include(a => a.Article)
                .Include(a => a.Storage)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (articleInstance == null)
            {
                return NotFound();
            }

            return View(articleInstance);
        }

        // GET: ArticleInstance/Create
        public async Task<IActionResult> Create()
        {
            ViewData["ArticleId"] = await GetArticlesSelectList();
            ViewData["StorageId"] = new SelectList(_context.KitchenStorages, "Id", "Name");
            return View();
        }

        // POST: ArticleInstance/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ArticleId,StorageId,Count")] ArticleInstance articleInstance)
        {
            if (ModelState.IsValid)
            {
                articleInstance.Id = Guid.NewGuid();
                _context.Add(articleInstance);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ArticleId"] = await GetArticlesSelectList(articleInstance.ArticleId);
            ViewData["StorageId"] = new SelectList(_context.KitchenStorages, "Id", "Name", articleInstance.StorageId);
            return View(articleInstance);
        }

        // GET: ArticleInstance/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var articleInstance = await _context.KitchenArticleInstances.Include(a => a.Article).FirstOrDefaultAsync(a => a.Id == id);
            if (articleInstance == null)
            {
                return NotFound();
            }
            ViewData["ArticleId"] = await GetArticlesSelectList(articleInstance.ArticleId);
            ViewData["StorageId"] = new SelectList(_context.KitchenStorages, "Id", "Name", articleInstance.StorageId);
            return View(articleInstance);
        }

        // POST: ArticleInstance/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,ArticleId,StorageId,Count")] ArticleInstance articleInstance)
        {
            if (id != articleInstance.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(articleInstance);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArticleInstanceExists(articleInstance.Id))
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
            ViewData["ArticleId"] = await GetArticlesSelectList(articleInstance.ArticleId);
            ViewData["StorageId"] = new SelectList(_context.KitchenStorages, "Id", "Name", articleInstance.StorageId);
            return View(articleInstance);
        }

        // GET: ArticleInstance/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var articleInstance = await _context.KitchenArticleInstances
                .Include(a => a.Article)
                .Include(a => a.Storage)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (articleInstance == null)
            {
                return NotFound();
            }

            return View(articleInstance);
        }

        // POST: ArticleInstance/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var articleInstance = await _context.KitchenArticleInstances.FindAsync(id);
            if (articleInstance != null)
            {
                _context.KitchenArticleInstances.Remove(articleInstance);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ArticleInstanceExists(Guid id)
        {
            return _context.KitchenArticleInstances.Any(e => e.Id == id);
        }


        private async Task<SelectList> GetArticlesSelectList(Guid? articleId = null)
        {
            var articles = await _context.KitchenArticles
                .AsNoTracking()
                .ToListAsync();

            var articlesSelList = new List<SelectListItem>();
            foreach (var article in articles)
            {
                string unitText = string.Empty;
                switch (article.Unit)
                {
                    case ArticleUnit.Pieces:
                        unitText = "Pieces";
                        break;
                    case ArticleUnit.Kg:
                        unitText = "kg";
                        break;
                    case ArticleUnit.Liters:
                        unitText = "l";
                        break;
                }

                articlesSelList.Add(new SelectListItem(article.Name + " (" + unitText + ")", article.Id.ToString()));
            }

            if (articleId != null)
                return new SelectList(articlesSelList, "Value", "Text", articleId);
            else
                return new SelectList(articlesSelList, "Value", "Text");
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
