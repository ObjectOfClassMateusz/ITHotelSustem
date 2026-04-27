using HotelSystemIndustry.Infrastructure;
using HotelSystemIndustry.Models.Kitchen;
using HotelSystemIndustry.ViewModels.Kitchen;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.DotNet.Scaffolding.Shared.ProjectModel;
using Microsoft.EntityFrameworkCore;

namespace HotelSystemIndustry.Controllers.Kitchen
{

    public class KitchenController : Controller
    {
        private HotelDbContext _context;
        
        public KitchenController(HotelDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles="KitchenEmployee")]
        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> PlaceOrder()
        {
            ViewBag.OrderTypes = new SelectList(_context.KitchenOrderTypes, "Id", "Name");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChooseOrderProducts(NewOrderViewModel model)
        {
            NewOrderNewProductViewModel newModel = new()
            {
                Order = model
            };

            var products = await _context.KitchenProducts.AsNoTracking().ToListAsync();
            ViewBag.Products = products;

            return View("AddOrderProduct", newModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrderProduct(NewOrderNewProductViewModel model)
        {
            if (model.NewProductId != null && model.NewProductCount != null)
            {
                if (model.Order.Products.Any(p => p.ProductId == model.NewProductId.Value))
                {
                    var product = model.Order.Products.First(p => p.ProductId == model.NewProductId.Value);
                    product.Count += model.NewProductCount.Value;
                }
                else
                {
                    model.Order.Products.Add(new ProductAndNumber { ProductId = model.NewProductId.Value, Count = model.NewProductCount.Value });
                }

                model.NewProductId = null;
            }

            var products = await _context.KitchenProducts.AsNoTracking().ToListAsync();
            ViewBag.Products = products;

            ModelState.Clear(); // Zresetuj zapamiętane wartości w Hiddenach

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyOrder(NewOrderNewProductViewModel model)
        {
            var orderTypes = await _context.KitchenOrderTypes.AsNoTracking().ToListAsync();
            ViewBag.OrderTypes = orderTypes;

            var products = await _context.KitchenProducts.AsNoTracking().ToListAsync();
            ViewBag.Products = products;

            return View(model.Order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitOrder([FromForm] NewOrderViewModel model)
        {
            var orderApi = new OrderApiController(_context)
            {
                ControllerContext = this.ControllerContext
            };

            var result = await orderApi.SubmitOrder(model);

            if (!result)
                return BadRequest("Invalid order type or invalid products!");

            return View("OrderSubmitSuccess");
        }


        [HttpGet]
        [Authorize(Roles="KitchenEmployee")]
        public async Task<IActionResult> RealiseOrderView()
        {
            var unrealisedOrders = await _context.KitchenOrders
                .AsNoTracking()
                .Where(p => p.RealisedTime == null)
                .Include(p => p.Type)
                .Include(p => p.Products)
                    !.ThenInclude(op => op.Product)
                .ToListAsync();

            return View(unrealisedOrders);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles="KitchenEmployee")]
        public async Task<IActionResult> MarkOrderRealised(Guid id)
        {
            var orderApi = new OrderApiController(_context)
            {
                ControllerContext = this.ControllerContext
            };

            var result = await orderApi.MarkOrderRealised(id);

            if (!result)
                return NotFound();

            return RedirectToAction("RealiseOrderView", "Kitchen");
        }


        [HttpGet]
        [Authorize(Roles="KitchenEmployee")]
        public async Task<IActionResult> CancelOrderView()
        {
            var unrealisedOrders = await _context.KitchenOrders
                .AsNoTracking()
                .Where(p => p.RealisedTime == null)
                .Include(p => p.Type)
                .Include(p => p.Products)
                    !.ThenInclude(op => op.Product)
                .ToListAsync();

            return View(unrealisedOrders);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles="KitchenEmployee")]
        public async Task<IActionResult> CancelOrderConfirmation(Guid id)
        {
            var order = await _context.KitchenOrders
                .AsNoTracking()
                .Where(o => o.Id == id)
                .Include(o => o.Type)
                .Include(o => o.Products)
                    !.ThenInclude(op => op.Product)
                .SingleAsync();

            return View(order);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles="KitchenEmployee")]
        public async Task<IActionResult> CancelOrder(Guid id)
        {
            var orderApi = new OrderApiController(_context)
            {
                ControllerContext = this.ControllerContext
            };

            var result = await orderApi.CancelOrder(id);

            if (!result)
                return NotFound();

            return RedirectToAction("CancelOrderView");
        }
    }

}