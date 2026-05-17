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
                    .FirstOrDefaultAsync(s => s.Id == item.SaleItemInstanceId);

                if (saleItemInstance == null)
                    return false;

                if (saleItemInstance.Count < item.Count)
                    return false;

                var purchaseItem = new PurchaseItem
                {
                    Id = Guid.NewGuid(),
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


        [HttpGet("[action]")]
        public async Task<List<PurchaseItem>> GetItemsToReturn()
        {
            var unreturnedRentItems = await _context.PurchaseItems
                .AsNoTracking()
                .Include(p => p.SaleItem)
                    .ThenInclude(si => si!.Type)
                .Where(p => p.SaleItem!.Type!.IsForRent && !p.HasBeenReturned)
                .ToListAsync();

            return unreturnedRentItems;
        }


        [HttpPost("[action]")]
        public async Task<bool> ReturnItem(Guid purchaseItemId, Guid magazineId)
        {
            var purchaseItem = await _context.PurchaseItems
                .Where(p => p.Id == purchaseItemId)
                .FirstOrDefaultAsync();

            if (purchaseItem == null)
                return false;

            var saleItemInstance = await _context.SaleItemInstances
                .Where(s => s.ItemId == purchaseItem.SaleItemId &&
                            s.Variant == purchaseItem.Variant &&
                            s.MagazineId == magazineId)
                .FirstOrDefaultAsync();

            if (saleItemInstance != null)
            {
                saleItemInstance.Count += purchaseItem.Count;
                _context.Update(saleItemInstance);

                purchaseItem.HasBeenReturned = true;
                _context.Update(purchaseItem);

                await _context.SaveChangesAsync();
            }
            else
            {
                var magazine = await _context.ShopMagazines
                    .Where(m => m.Id == magazineId)
                    .FirstOrDefaultAsync();

                if (magazine == null)
                    return false;

                saleItemInstance = new SaleItemInstance
                {
                    Id = Guid.NewGuid(),
                    ItemId = purchaseItem.SaleItemId,
                    MagazineId = magazineId,
                    Magazine = magazine,
                    Variant = purchaseItem.Variant,
                    Count = purchaseItem.Count,
                    Price = purchaseItem.UnitPrice
                };
                _context.Add(saleItemInstance);

                purchaseItem.HasBeenReturned = true;
                _context.Update(purchaseItem);

                await _context.SaveChangesAsync();
            }

            return true;
        }


        [HttpPost("[action]")]
        public async Task<bool> AcceptItemsDelivery(IList<TradingDeliveryItem> items)
        {
            foreach (var item in items)
            {
                var expireDate = item.ExpireDate;
                if (expireDate != null)
                    expireDate = expireDate.Value.ToUniversalTime();

                var saleItem = await _context.SaleItems.FirstOrDefaultAsync(si => si.Id == item.SaleItemId);
                var magazine = await _context.ShopMagazines.FirstOrDefaultAsync(m => m.Id == item.MagazineId);

                if (saleItem == null || magazine == null)
                    return false;

                var saleItemInstance = await _context.SaleItemInstances
                    .Where(si => si.ItemId == item.SaleItemId && si.MagazineId == item.MagazineId &&
                           si.Variant == item.Variant && si.Price == item.Price && si.ExpireDate == expireDate)
                    .FirstOrDefaultAsync();

                if (saleItemInstance != null)
                {
                    saleItemInstance.Count += item.Count;
                    _context.Update(saleItemInstance);
                }
                else
                {
                    saleItemInstance = new SaleItemInstance
                    {
                        Id = Guid.NewGuid(),
                        ItemId = item.SaleItemId,
                        Item = saleItem,
                        MagazineId = item.MagazineId,
                        Magazine = magazine,
                        Variant = item.Variant ?? string.Empty,
                        ExpireDate = expireDate,
                        Price = item.Price,
                        Count = item.Count
                    };
                    _context.Add(saleItemInstance);
                }
            }


            await _context.SaveChangesAsync();
            
            return true;
        }


        [HttpPost("[action]")]
        public async Task<List<SaleItemInstance>> PruneExpiredItems()
        {
            DateTime today = DateTime.UtcNow.Date;

            var expiredItems = await _context.SaleItemInstances
                .Where(s => s.ExpireDate != null && s.ExpireDate <= today)
                .Include(s => s.Item)
                    .ThenInclude(s => s!.Type)
                .Include(s => s.Magazine)
                .ToListAsync();

            if (expiredItems.Count > 0)
            {
                _context.SaleItemInstances.RemoveRange(expiredItems);
                await _context.SaveChangesAsync();
            }

            return expiredItems;
        }
    }

}