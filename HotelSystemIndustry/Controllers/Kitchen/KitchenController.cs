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
            if (!ModelState.IsValid)
            {
                ViewBag.OrderTypes = new SelectList(_context.KitchenOrderTypes, "Id", "Name");
                return View("PlaceOrder");
            }

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
            if (ModelState.IsValid)
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
            }

            var products = await _context.KitchenProducts.AsNoTracking().ToListAsync();
            ViewBag.Products = products;

            ModelState.Clear(); // Zresetuj zapamiętane wartości w Hiddenach

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveOrderProduct(NewOrderNewProductViewModel model, int index)
        {
            if (ModelState.IsValid && index >= 0 && index < model.Order.Products.Count)
            {
                model.Order.Products.RemoveAt(index);
            }

            var products = await _context.KitchenProducts.AsNoTracking().ToListAsync();
            ViewBag.Products = products;

            ModelState.Clear(); // Zresetuj zapamiętane wartości w Hiddenach

            return View("AddOrderProduct", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyOrder(NewOrderNewProductViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid order data!");

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
            if (!ModelState.IsValid)
                return BadRequest("Invalid order data!");

            var orderApi = new OrderApiController(_context)
            {
                ControllerContext = this.ControllerContext
            };

            Guid result = await orderApi.SubmitOrder(model);

            if (result == Guid.Empty)
                return BadRequest("Invalid order type or invalid products!");

            return RedirectToAction("OrderSubmitSuccess");
        }


        [HttpGet]
        public async Task<IActionResult> OrderSubmitSuccess()
        {
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


        [HttpGet]
        [Authorize(Roles="KitchenEmployee")]
        public async Task<IActionResult> HandleDelivery()
        {
            var articles = await _context.KitchenArticles.AsNoTracking().ToListAsync();
            ViewBag.Articles = articles;
            ViewBag.Storages = await _context.KitchenStorages.AsNoTracking().ToListAsync();
            ViewBag.ArticlesSelectList = await GetArticlesSelectList(articles);
            ViewBag.StorageSelectList = new SelectList(_context.KitchenStorages, "Id", "Name");
            return View(new KitchenDeliveryArticleViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles="KitchenEmployee")]
        public async Task<IActionResult> AddDeliveryArticle(KitchenDeliveryArticleViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.DeliveredArticles.Add(model.ToAdd);
                model.ToAdd = new KitchenArticleDelivery();
            }

            ModelState.Clear();

            var articles = await _context.KitchenArticles.AsNoTracking().ToListAsync();
            ViewBag.Articles = articles;
            ViewBag.Storages = await _context.KitchenStorages.AsNoTracking().ToListAsync();
            ViewBag.ArticlesSelectList = await GetArticlesSelectList(articles);
            ViewBag.StorageSelectList = new SelectList(_context.KitchenStorages, "Id", "Name");
            return View("HandleDelivery", model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles="KitchenEmployee")]
        public async Task<IActionResult> RegisterDeliveredArticles(KitchenDeliveryArticleViewModel model)
        {
            if (ModelState.IsValid)
            {
                KitchenApiController apiController = new KitchenApiController(_context)
                {
                    ControllerContext = this.ControllerContext
                };

                var result = await apiController.RegisterDeliveredArticles(model.DeliveredArticles);
                if (!result)
                    return BadRequest("Invalid delivery data!");
            }
            else
                return BadRequest("Invalid delivery data!");

            return RedirectToAction("DeliveryRegisterSuccess");
        }


        [HttpGet]
        [Authorize(Roles="KitchenEmployee")]
        public async Task<IActionResult> DeliveryRegisterSuccess()
        {
            return View();
        }


        private async Task<List<SelectListItem>> GetArticlesSelectList(IList<KitchenArticle> articles)
        {
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
            return articlesSelList;
        }
    }

}