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
            Guid currentHotel = await GetCurrentHotelId();

            ViewBag.HotelChangePartialHotelList = new SelectList(_context.Hotels, "Id", "Name", currentHotel);
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> SellOrRentItemsView()
        {
            SellOrRentItemsViewModel model = new();

            Guid currentHotel = await GetCurrentHotelId();

            var magazine = await _context.ShopMagazines
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.HotelId == currentHotel);
            var magazineId = magazine != null ? magazine.Id : Guid.Empty;
            model.Items.MagazineId = magazineId;

            var items = await GetSaleItemInstances();

            ViewBag.SaleItemInstances = items;

            ViewBag.ShopPointList = GetShopPointsSelectList(currentHotel);
            ViewBag.MagazineList = GetMagazineSelectList(currentHotel, magazineId);

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RefreshSaleOrRentItemsView(SellOrRentItemsViewModel model)
        {
            Guid currentHotel = await GetCurrentHotelId();

            var items = await GetSaleItemInstances();

            ViewBag.SaleItemInstances = items;

            ViewBag.ShopPointList = GetShopPointsSelectList(currentHotel, model.Items.ShopPointId);
            ViewBag.MagazineList = GetMagazineSelectList(currentHotel, model.Items.MagazineId);

            ModelState.Clear();

            return View("SellOrRentItemsView", model);
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

            Guid currentHotel = await GetCurrentHotelId();

            var items = await GetSaleItemInstances();

            ViewBag.SaleItemInstances = items;

            ViewBag.ShopPointList = GetShopPointsSelectList(currentHotel, model.Items.ShopPointId);
            ViewBag.MagazineList = GetMagazineSelectList(currentHotel, model.Items.MagazineId);

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

            Guid currentHotel = await GetCurrentHotelId();

            var items = await GetSaleItemInstances();

            ViewBag.SaleItemInstances = items;

            ViewBag.ShopPointList = GetShopPointsSelectList(currentHotel, model.Items.ShopPointId);
            ViewBag.MagazineList = GetMagazineSelectList(currentHotel, model.Items.MagazineId);

            ModelState.Clear();

            return View("SellOrRentItemsView", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyPurchase(SellOrRentItemsViewModel model)
        {
            var items = await GetSaleItemInstances();
            ViewBag.SaleItemInstances = items;

            if (model.Items.ShopPointId != Guid.Empty)
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
            Guid currentHotel = await GetCurrentHotelId();

            var items = await GetSaleItemInstances();

            ViewBag.SaleItemInstances = items;

            ViewBag.ShopPointList = GetShopPointsSelectList(currentHotel, model.ShopPointId);
            ViewBag.MagazineList = GetMagazineSelectList(currentHotel, model.MagazineId);

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
            Guid currentHotel = await GetCurrentHotelId();

            var unreturnedRentItems = await _context.PurchaseItems
                .AsNoTracking()
                .Include(p => p.SaleItem)
                    .ThenInclude(si => si!.Type)
                .Include(p => p.Purchase)
                    .ThenInclude(p => p!.ShopPoint)
                .Where(p => p.Purchase!.ShopPoint!.HotelId == currentHotel && p.SaleItem!.Type!.IsForRent && !p.HasBeenReturned)
                .ToListAsync();

            ViewBag.Items = unreturnedRentItems;

            return View();
        }


        [HttpGet("[controller]/[action]/{purchaseItemId}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> ReturnItemMenu(Guid purchaseItemId)
        {
            Guid currentHotel = await GetCurrentHotelId();

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
                .Include(s => s.Magazine)
                .Where(s => s.Magazine!.HotelId == currentHotel && s.ItemId == purchaseItem.SaleItemId && s.Variant == purchaseItem.Variant)
                .ToListAsync();

            // Find magazines that are not on the list above
            var magazines = await _context.ShopMagazines
                .AsNoTracking()
                .Where(m => m.HotelId == currentHotel)
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
            Guid currentHotel = await GetCurrentHotelId();

            var items = await _context.SaleItems.AsNoTracking().ToListAsync();
            var magazines = await _context.ShopMagazines
                .AsNoTracking()
                .Where(m => m.HotelId == currentHotel)
                .ToListAsync();

            var itemInstances = await _context.SaleItemInstances
                .AsNoTracking()
                .Include(s => s.Magazine)
                .Where(s => s.Magazine!.HotelId == currentHotel)
                .ToListAsync();

            var itemSelectList = new SelectList(_context.SaleItems, "Id", "Name");
            var magazineSelectList = new SelectList(magazines, "Id", "Location");

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


            Guid currentHotel = await GetCurrentHotelId();

            var items = await _context.SaleItems.AsNoTracking().ToListAsync();
            var magazines = await _context.ShopMagazines
                .AsNoTracking()
                .Where(m => m.HotelId == currentHotel)
                .ToListAsync();

            var itemInstances = await _context.SaleItemInstances
                .AsNoTracking()
                .Include(s => s.Magazine)
                .Where(s => s.Magazine!.HotelId == currentHotel)
                .ToListAsync();

            var itemSelectList = new SelectList(_context.SaleItems, "Id", "Name");
            var magazineSelectList = new SelectList(magazines, "Id", "Location");

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
            Guid currentHotel = await GetCurrentHotelId();
            DateTime today = DateTime.UtcNow.Date;

            var expiredItems = await _context.SaleItemInstances
                .AsNoTracking()
                .Include(s => s.Item)
                .Include(s => s.Magazine)
                .Where(s => s.Magazine!.HotelId == currentHotel && s.ExpireDate != null && s.ExpireDate <= today)
                .ToListAsync();

            ViewBag.ExpiredItems = expiredItems;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmExpiredItemsPrunning()
        {
            Guid currentHotel = await GetCurrentHotelId();
            DateTime today = DateTime.UtcNow.Date;

            var expiredItems = await _context.SaleItemInstances
                .AsNoTracking()
                .Include(s => s.Item)
                .Include(s => s.Magazine)
                .Where(s => s.Magazine!.HotelId == currentHotel && s.ExpireDate != null && s.ExpireDate <= today)
                .ToListAsync();

            TradingApiController apiController = new TradingApiController(_context)
            {
                ControllerContext = this.ControllerContext
            };

            await apiController.PruneExpiredItems(currentHotel);

            return RedirectToAction("PruneExpiredItems");
        }


        private SelectList GetShopPointsSelectList(Guid hotelId, Guid? shopPointId = null)
        {
            List<SelectListItem> items = _context.ShopPoints
                .Where(sp => sp.HotelId == hotelId)
                .Select(sp => new SelectListItem(sp.Location, sp.Id.ToString()))
                .ToList();
            
            if (shopPointId != null && shopPointId != Guid.Empty)
                return new SelectList(items, "Value", "Text", shopPointId.Value.ToString());
            else
                return new SelectList(items, "Value", "Text");
        }

        private SelectList GetMagazineSelectList(Guid hotelId, Guid? magazineId = null)
        {
            List<SelectListItem> items = _context.ShopMagazines
                .Where(sm => sm.HotelId == hotelId)
                .Select(sp => new SelectListItem(sp.Location, sp.Id.ToString()))
                .ToList();
            
            if (magazineId != null && magazineId != Guid.Empty)
                return new SelectList(items, "Value", "Text", magazineId.Value.ToString());
            else
                return new SelectList(items, "Value", "Text");
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