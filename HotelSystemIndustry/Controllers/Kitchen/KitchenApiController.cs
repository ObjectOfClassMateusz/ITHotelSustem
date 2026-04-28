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