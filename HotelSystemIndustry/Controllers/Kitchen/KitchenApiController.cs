using HotelSystemIndustry.Infrastructure;
using HotelSystemIndustry.Models.Kitchen;
using HotelSystemIndustry.ViewModels.Kitchen;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelSystemIndustry.Controllers.Kitchen
{
    [ApiController]
    [Route("api/[controller]")]
    public class KitchenApiController : Controller
    {
        private HotelDbContext _context;
        
        public KitchenApiController(HotelDbContext context)
        {
            _context = context;
        }


        [HttpGet("[action]")]
        [Authorize(Roles="KitchenEmployee")]
        public async Task<List<KitchenArticle>> GetArticleList()
        {
            var articles = await _context.KitchenArticles
                .AsNoTracking()
                .Include(a => a.Type)
                .ToListAsync();
            return articles;
        }


        [HttpGet("[action]/{articleId}")]
        [Authorize(Roles="KitchenEmployee")]
        public async Task<List<ArticleInstance>> GetArticleInstanceList(Guid articleId)
        {
            var articleInstances = await _context.KitchenArticleInstances
                .AsNoTracking()
                .Where(ai => ai.ArticleId == articleId)
                .Include(ai => ai.Storage)
                .ToListAsync();
            return articleInstances;
        }


        [HttpGet("[action]")]
        [Authorize(Roles="KitchenEmployee")]
        public async Task<List<Storage>> GetStorageList()
        {
            var storages = await _context.KitchenStorages
                .AsNoTracking()
                .ToListAsync();
            return storages;
        }


        [HttpGet("[action]")]
        [Authorize(Roles="KitchenEmployee")]
        public async Task<List<KitchenProduct>> GetProductList()
        {
            var products = await _context.KitchenProducts
                .AsNoTracking()
                .ToListAsync();
            return products;
        }


        [HttpGet("[action]/{id}")]
        [Authorize(Roles="KitchenEmployee")]
        public async Task<List<KitchenRecipe>> GetRecipesForProduct(Guid id)
        {
            var recipes = await _context.KitchenRecipes
                .AsNoTracking()
                .Where(r => r.OutcomeProductId == id)
                .Include(r => r.OutcomeProduct)
                .Include(r => r.Ingredients)
                    !.ThenInclude(ri => ri.Article)
                        .ThenInclude(a => a!.Type)
                .ToListAsync();
            return recipes;
        }


        [HttpPost("[action]")]
        [Authorize(Roles="KitchenEmployee")]
        public async Task<bool> TakeArticleInstances(Guid instanceId, decimal count)
        {
            if (count < 0.0m)
                return false;

            var artInstance = await _context.KitchenArticleInstances
                .Where(ai => ai.Id == instanceId)
                .FirstOrDefaultAsync();

            if (artInstance == null)
                return false;

            
            if (artInstance.Count == count)
            {
                _context.Remove(artInstance);
                await _context.SaveChangesAsync();
            }
            else if (artInstance.Count > count)
            {
                artInstance.Count -= count;
                _context.Update(artInstance);
                await _context.SaveChangesAsync();
            }
            else
            {
                return false;
            }

            return true;
        }


        [HttpPost("[action]")]
        [Authorize(Roles="KitchenEmployee")]
        public async Task<bool> RegisterDeliveredArticles(IList<KitchenArticleDelivery> delivery)
        {
            foreach (var deliveredArticle in delivery)
            {
                var article = await _context.KitchenArticles.FirstOrDefaultAsync(a => a.Id == deliveredArticle.ArticleId);
                var storage = await _context.KitchenStorages.FirstOrDefaultAsync(s => s.Id == deliveredArticle.ToStorageId);

                if (article == null)
                    return false;

                if (storage == null)
                    return false;
                
                var articleInstance = await _context.KitchenArticleInstances
                    .FirstOrDefaultAsync(a => a.ArticleId == deliveredArticle.ArticleId && a.StorageId == deliveredArticle.ToStorageId);
                
                if (articleInstance == null)
                {
                    articleInstance = new ArticleInstance
                    {
                        Id = Guid.NewGuid(),
                        ArticleId = deliveredArticle.ArticleId,
                        StorageId = deliveredArticle.ToStorageId,
                        Count = deliveredArticle.AddCount
                    };
                    _context.Add(articleInstance);
                }
                else
                {
                    articleInstance.Count += deliveredArticle.AddCount;
                    _context.Update(articleInstance);
                }

                await _context.SaveChangesAsync();
            }

            return true;
        }

    }
}