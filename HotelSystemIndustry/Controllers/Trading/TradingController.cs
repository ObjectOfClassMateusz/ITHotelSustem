using System.Collections.ObjectModel;
using HotelSystemIndustry.Infrastructure;
using HotelSystemIndustry.Models.Trading;
using HotelSystemIndustry.ViewModels.Trading;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

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
                var item = model.Items.Items.FirstOrDefault(i => i.SaleItemId == model.NewItemId);
                if (item != null)
                {
                    item.Count += model.NewItemCount;
                }
                else
                {
                    model.Items.Items.Add(new SaleItemAndCount
                    {
                        SaleItemId = model.NewItemId.Value,
                        Count = model.NewItemCount
                    });
                }

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
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> AcceptDeliveryView()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> PruneExpiredItems()
        {
            return View();
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