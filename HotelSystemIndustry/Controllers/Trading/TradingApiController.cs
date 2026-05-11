using System.Collections.ObjectModel;
using HotelSystemIndustry.Infrastructure;
using HotelSystemIndustry.Models.Trading;
using HotelSystemIndustry.ViewModels.Trading;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelSystemIndustry.Controllers.Trading
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles="TradingEmployee")]
    public class TradingApiController : Controller
    {
        private HotelDbContext _context;


        public TradingApiController(HotelDbContext context)
        {
            _context = context;
        }


        [HttpGet("[action]")]
        public async Task<IList<SaleItemInstance>> GetItemsForSale()
        {
            var items = await _context.SaleItemInstances
                .AsNoTracking()
                .Include(s => s.Item)
                    .ThenInclude(s => s!.Type)
                .Include(s => s.Magazine)
                .ToListAsync();

            return items;
        }


        [HttpGet("[action]")]
        public async Task<IList<ShopPoint>> GetShopPoints()
        {
            var shopPoints = await _context.ShopPoints
                .AsNoTracking()
                .ToListAsync();

            return shopPoints;
        }


        [HttpGet("[action]")]
        public async Task<IList<ShopMagazine>> GetShopMagazines()
        {
            var magazines = await _context.ShopMagazines
                .AsNoTracking()
                .ToListAsync();
            
            return magazines;
        }


        [HttpPost("[action]")]
        public async Task<bool> RegisterPurchase(SellOrRentItems purchaseItems)
        {
            Purchase purchase = new Purchase
            {
                Id = Guid.NewGuid(),
                TransactionDate = DateTime.UtcNow,
                Items = new Collection<PurchaseItem>()
            };
            if (purchaseItems.ShopPointId != null && purchaseItems.ShopPointId != Guid.Empty)
                purchase.ShopPointId = purchaseItems.ShopPointId;


            foreach (var item in purchaseItems.Items)
            {
                var saleItemInstance = await _context.SaleItemInstances
                    .Include(s => s.Item)
                        .ThenInclude(s => s!.Type)
                    .FirstOrDefaultAsync(s => s.Id == item.SaleItemId);

                if (saleItemInstance == null)
                    return false;

                if (saleItemInstance.Count < item.Count)
                    return false;

                var purchaseItem = new PurchaseItem
                {
                    PurchaseId = purchase.Id,
                    Purchase = purchase,
                    SaleItemId = saleItemInstance.ItemId,
                    SaleItem = saleItemInstance.Item,
                    Count = item.Count,
                    UnitPrice = saleItemInstance.Price,
                    Variant = saleItemInstance.Variant
                };
                purchase.Items.Add(purchaseItem);

                if (saleItemInstance.Count == item.Count && !saleItemInstance.Item!.Type!.IsForRent)
                {
                    _context.Remove(saleItemInstance);
                }
                else
                {
                    saleItemInstance.Count -= item.Count;
                    _context.Update(saleItemInstance);
                }
            }

            _context.Add(purchase);
            foreach (var purchaseItem in purchase.Items)
                _context.Add(purchaseItem);

            await _context.SaveChangesAsync();

            return true;
        }
    }

}