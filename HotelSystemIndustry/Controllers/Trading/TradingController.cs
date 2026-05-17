using System.Collections.ObjectModel;
using HotelSystemIndustry.Infrastructure;
using HotelSystemIndustry.Models.Trading;
using HotelSystemIndustry.ViewModels.Trading;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;

namespace HotelSystemIndustry.Controllers.Trading
{
    [Authorize(Roles="TradingEmployee")]
    public class TradingController : Controller
    {

        private HotelDbContext _context;
        

        public TradingController(HotelDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> SellOrRentItemsView()
        {
            SellOrRentItemsViewModel model = new();

            var items = await GetSaleItemInstances();

            ViewBag.SaleItemInstances = items;

            ViewBag.ShopPointList = GetShopPointsSelectList();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSaleItem(SellOrRentItemsViewModel model)
        {
            if (model.NewItemId != null && model.NewItemId != Guid.Empty && model.NewItemCount > 0)
            {
                var saleItemInstance = await _context.SaleItemInstances.FirstOrDefaultAsync(si => si.Id == model.NewItemId);
                if (saleItemInstance == null)
                    return BadRequest("Invalid sale item instance ID!");

                var item = model.Items.Items.FirstOrDefault(i => i.SaleItemInstanceId == model.NewItemId);
                if (item != null)
                {
                    item.Count += model.NewItemCount;
                }
                else
                {
                    item = new SaleItemAndCount
                    {
                        SaleItemInstanceId = model.NewItemId.Value,
                        Count = model.NewItemCount
                    };
                    model.Items.Items.Add(item);
                }

                if (item.Count > saleItemInstance.Count)
                    item.Count = saleItemInstance.Count;

                model.NewItemId = Guid.Empty;
            }

            var items = await GetSaleItemInstances();

            ViewBag.SaleItemInstances = items;

            ViewBag.ShopPointList = GetShopPointsSelectList(model.Items.ShopPointId);

            ModelState.Clear();

            return View("SellOrRentItemsView", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveSaleItem(SellOrRentItemsViewModel model, int index)
        {
            if (index >= 0 && index < model.Items.Items.Count)
            {
                model.Items.Items.RemoveAt(index);
            }

            var items = await GetSaleItemInstances();

            ViewBag.SaleItemInstances = items;

            ViewBag.ShopPointList = GetShopPointsSelectList(model.Items.ShopPointId);

            ModelState.Clear();

            return View("SellOrRentItemsView", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyPurchase(SellOrRentItemsViewModel model)
        {
            var items = await GetSaleItemInstances();
            ViewBag.SaleItemInstances = items;

            if (model.Items.ShopPointId != null && model.Items.ShopPointId != Guid.Empty)
            {
                var shopPoint = await _context.ShopPoints.FirstOrDefaultAsync(sp => sp.Id == model.Items.ShopPointId);
                if (shopPoint == null)
                    return BadRequest("Invalid shop point ID!");

                ViewBag.ShopPointText = shopPoint.Location;
            }
            else
                ViewBag.ShopPointText = "—";

            ModelState.Clear();

            return View(model.Items);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReturnToSellOrRentItemsView(SellOrRentItems model)
        {
            var items = await GetSaleItemInstances();

            ViewBag.SaleItemInstances = items;

            ViewBag.ShopPointList = GetShopPointsSelectList(model.ShopPointId);

            return View("SellOrRentItemsView", new SellOrRentItemsViewModel{Items = model});
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitPurchase(SellOrRentItems model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid form data");


            TradingApiController tradingApi = new TradingApiController(_context)
            {
                ControllerContext = this.ControllerContext
            };
            
            var result = await tradingApi.RegisterPurchase(model);

            if (!result)
                return BadRequest("Invalid items or not enough of an item in the purchase!");
            
            return View("PurchaseSubmitSuccess");
        }


        [HttpGet]
        public async Task<IActionResult> AcceptReturnView()
        {
            var unreturnedRentItems = await _context.PurchaseItems
                .AsNoTracking()
                .Include(p => p.SaleItem)
                    .ThenInclude(si => si!.Type)
                .Where(p => p.SaleItem!.Type!.IsForRent && !p.HasBeenReturned)
                .ToListAsync();

            ViewBag.Items = unreturnedRentItems;

            return View();
        }


        [HttpGet("[action]/{purchaseItemId}")]
        public async Task<IActionResult> ReturnItemMenu(Guid purchaseItemId)
        {
            var purchaseItem = await _context.PurchaseItems
                .AsNoTracking()
                .Where(p => p.Id == purchaseItemId)
                .Include(p => p.SaleItem)
                    .ThenInclude(s => s!.Type)
                .FirstOrDefaultAsync();
            if (purchaseItem == null)
                return BadRequest("Invalid purchase item to return!");

            // Find instances of the same item of the same variant
            var instances = await _context.SaleItemInstances
                .AsNoTracking()
                .Where(s => s.ItemId == purchaseItem.SaleItemId && s.Variant == purchaseItem.Variant)
                .Include(s => s.Magazine)
                .ToListAsync();

            // Find magazines that are not on the list above
            var magazines = await _context.ShopMagazines
                .AsNoTracking()
                .ToListAsync();

            for (var i = 0; i < magazines.Count;)
            {
                if (instances.Any(s => s.MagazineId == magazines[i].Id))
                    magazines.RemoveAt(i);
                else
                    i++;
            }

            var magazineSelectList = magazines.Select(m => new SelectListItem(m.Location.ToString(), m.Id.ToString()));
            

            ViewBag.PurchaseItem = purchaseItem;
            ViewBag.SaleItemInstances = instances;
            ViewBag.MagazineSelectList = magazineSelectList;
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmItemReturn(Guid purchaseItemId, Guid magazineId)
        {
            TradingApiController apiController = new TradingApiController(_context)
            {
                ControllerContext = this.ControllerContext
            };
            await apiController.ReturnItem(purchaseItemId, magazineId);

            return RedirectToAction("AcceptReturnView");
        }


        [HttpGet]
        public async Task<IActionResult> AcceptDeliveryView()
        {
            var items = await _context.SaleItems.AsNoTracking().ToListAsync();
            var magazines = await _context.ShopMagazines.AsNoTracking().ToListAsync();

            var itemInstances = await _context.SaleItemInstances
                .AsNoTracking()
                .ToListAsync();

            var itemSelectList = new SelectList(_context.SaleItems, "Id", "Name");
            var magazineSelectList = new SelectList(_context.ShopMagazines, "Id", "Location");

            ViewBag.Items = items;
            ViewBag.ItemInstances = itemInstances;
            ViewBag.Magazines = magazines;
            ViewBag.ItemSelectList = itemSelectList;
            ViewBag.MagazineSelectList = magazineSelectList;
            return View(new AcceptTradingDeliveryViewModel());
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddDeliveryItem(AcceptTradingDeliveryViewModel model)
        {
            if (model.NewItem.SaleItemId != Guid.Empty && model.NewItem.MagazineId != Guid.Empty && model.NewItem.Count > 0)
            {
                model.Items.Add(model.NewItem);

                model.NewItem = new();
            }

            ModelState.Clear();


            var items = await _context.SaleItems.AsNoTracking().ToListAsync();
            var magazines = await _context.ShopMagazines.AsNoTracking().ToListAsync();

            var itemInstances = await _context.SaleItemInstances
                .AsNoTracking()
                .ToListAsync();

            var itemSelectList = new SelectList(_context.SaleItems, "Id", "Name");
            var magazineSelectList = new SelectList(_context.ShopMagazines, "Id", "Location");

            ViewBag.Items = items;
            ViewBag.ItemInstances = itemInstances;
            ViewBag.Magazines = magazines;
            ViewBag.ItemSelectList = itemSelectList;
            ViewBag.MagazineSelectList = magazineSelectList;

            return View("AcceptDeliveryView", model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitDelivery(AcceptTradingDeliveryViewModel model)
        {
            TradingApiController apiController = new TradingApiController(_context)
            {
                ControllerContext = this.ControllerContext
            };

            var result = await apiController.AcceptItemsDelivery(model.Items);

            if (!result)
                return BadRequest("Invalid delivery data!");
            
            return View("DeliverySuccess");
        }


        [HttpGet]
        public async Task<IActionResult> PruneExpiredItems()
        {
            DateTime today = DateTime.UtcNow.Date;

            var expiredItems = await _context.SaleItemInstances
                .AsNoTracking()
                .Where(s => s.ExpireDate != null && s.ExpireDate <= today)
                .Include(s => s.Item)
                .Include(s => s.Magazine)
                .ToListAsync();

            ViewBag.ExpiredItems = expiredItems;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmExpiredItemsPrunning()
        {
            DateTime today = DateTime.UtcNow.Date;

            var expiredItems = await _context.SaleItemInstances
                .Where(s => s.ExpireDate != null && s.ExpireDate <= today)
                .Include(s => s.Item)
                .Include(s => s.Magazine)
                .ToListAsync();

            TradingApiController apiController = new TradingApiController(_context)
            {
                ControllerContext = this.ControllerContext
            };

            await apiController.PruneExpiredItems();

            return RedirectToAction("PruneExpiredItems");
        }


        private SelectList GetShopPointsSelectList(Guid? shopPointId = null)
        {
            List<SelectListItem> items = _context.ShopPoints
                .Select(sp => new SelectListItem(sp.Location, sp.Id.ToString()))
                .ToList();

            items.Add(new SelectListItem("-- None --", Guid.Empty.ToString()));
            
            if (shopPointId != null && shopPointId != Guid.Empty)
                return new SelectList(items, "Value", "Text", shopPointId.Value.ToString());
            else
                return new SelectList(items, "Value", "Text", Guid.Empty.ToString());
        }

        private async Task<List<SaleItemInstance>> GetSaleItemInstances()
        {
            var items = await _context.SaleItemInstances
                .AsNoTracking()
                .Include(s => s.Item)
                    .ThenInclude(s => s!.Type)
                .Include(s => s.Magazine)
                .OrderBy(s => s.Item!.Name)
                .ToListAsync();

            return items;
        }
    }

}