using HotelSystemIndustry.Infrastructure;
using HotelSystemIndustry.Models;
using HotelSystemIndustry.Models.Kitchen;
using HotelSystemIndustry.ViewModels.Kitchen;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelSystemIndustry.Controllers.Kitchen
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles="KitchenEmployee")]
    public class KitchenApiController : Controller
    {
        private HotelDbContext _context;
        
        public KitchenApiController(HotelDbContext context)
        {
            _context = context;
        }

        [HttpGet("[action]")]
        public async Task<IList<Hotel>> GetHotels()
        {
            return await _context.Hotels.ToListAsync();
        }


        [HttpGet("[action]")]
        public async Task<List<KitchenArticle>> GetArticleList()
        {
            var articles = await _context.KitchenArticles
                .AsNoTracking()
                .Include(a => a.Type)
                .ToListAsync();
            return articles;
        }


        [HttpGet("[action]/{hotelId}/{articleId}")]
        public async Task<List<ArticleInstance>> GetArticleInstanceList(Guid hotelId, Guid articleId)
        {
            var articleInstances = await _context.KitchenArticleInstances
                .AsNoTracking()
                .Include(ai => ai.Storage)
                .Where(ai => ai.ArticleId == articleId && ai.Storage!.HotelId == hotelId)
                .ToListAsync();
            return articleInstances;
        }


        [HttpGet("[action]/{hotelId}")]
        public async Task<List<Storage>> GetStorageList(Guid hotelId)
        {
            var storages = await _context.KitchenStorages
                .AsNoTracking()
                .Where(s => s.HotelId == hotelId)
                .ToListAsync();
            return storages;
        }


        [HttpGet("[action]")]
        public async Task<List<KitchenProduct>> GetProductList()
        {
            var products = await _context.KitchenProducts
                .AsNoTracking()
                .ToListAsync();
            return products;
        }


        [HttpGet("[action]/{id}")]
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