using HotelSystemIndustry.Infrastructure;
using HotelSystemIndustry.Models.Kitchen;
using HotelSystemIndustry.Services;
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
        private IExternalRecipeService _recipeService;
        
        public KitchenController(HotelDbContext context, IExternalRecipeService recipeService)
        {
            _context = context;
            _recipeService = recipeService;
        }

        [HttpGet]
        [Authorize(Roles="KitchenEmployee")]
        public async Task<IActionResult> Index()
        {
            Guid currentHotel = await GetCurrentHotelId();

            ViewBag.HotelChangePartialHotelList = new SelectList(_context.Hotels, "Id", "Name", currentHotel);
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> PlaceOrder()
        {
            ViewBag.OrderTypes = new SelectList(_context.KitchenOrderTypes, "Id", "Name");

            NewOrderViewModel model = new()
            {
                HotelId = await GetCurrentHotelId()
            };
            return View(model);
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

            var hotel = await _context.Hotels.AsNoTracking().FirstOrDefaultAsync(h => h.Id == model.Order.HotelId);
            if (hotel == null)
                return BadRequest("Invalid hotel ID!");

            ViewBag.HotelName = hotel.Name;

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
            Guid currentHotel = await GetCurrentHotelId();

            var unrealisedOrders = await _context.KitchenOrders
                .AsNoTracking()
                .Where(p => p.RealisedTime == null && p.HotelId == currentHotel)
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
            Guid currentHotel = await GetCurrentHotelId();

            var unrealisedOrders = await _context.KitchenOrders
                .AsNoTracking()
                .Where(p => p.RealisedTime == null && p.HotelId == currentHotel)
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
            Guid currentHotel = await GetCurrentHotelId();

            var articles = await _context.KitchenArticles.AsNoTracking().ToListAsync();
            var storages = await _context.KitchenStorages.AsNoTracking().Where(s => s.HotelId == currentHotel).ToListAsync();

            ViewBag.Articles = articles;
            ViewBag.Storages = storages;
            ViewBag.ArticlesSelectList = await GetArticlesSelectList(articles);
            ViewBag.StorageSelectList = new SelectList(storages, "Id", "Name");
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

            Guid currentHotel = await GetCurrentHotelId();

            var articles = await _context.KitchenArticles.AsNoTracking().ToListAsync();
            var storages = await _context.KitchenStorages.AsNoTracking().Where(s => s.HotelId == currentHotel).ToListAsync();

            ViewBag.Articles = articles;
            ViewBag.Storages = storages;
            ViewBag.ArticlesSelectList = await GetArticlesSelectList(articles);
            ViewBag.StorageSelectList = new SelectList(storages, "Id", "Name");
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


        [HttpGet]
        [Authorize(Roles="KitchenEmployee")]
        public async Task<IActionResult> ChooseProductCookingHelp()
        {
            List<KitchenRecipe> recipes = await _context.KitchenRecipes
                .Include(kr => kr.OutcomeProduct)
                .Include(kr => kr.Ingredients)
                    !.ThenInclude(kri => kri.Article)
                .ToListAsync();

            List<ChooseProductCookingHelpViewModel> productList = new();
            foreach (var recipe in recipes)
            {
                ChooseProductCookingHelpViewModel? product = productList.SingleOrDefault(p => p.ProductId == recipe.OutcomeProductId);

                if (product == null)
                {
                    product = new ChooseProductCookingHelpViewModel
                    {
                        ProductId = recipe.OutcomeProductId,
                        ProductName = recipe.OutcomeProduct!.Name
                    };

                    productList.Add(product);
                }

                product.ProductRecipes.Add(recipe);
            }

            ViewBag.ProductRecipes = productList;

            return View();
        }


        [HttpGet]
        [Authorize(Roles="KitchenEmployee")]
        public async Task<IActionResult> CookWithRecipe(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("CookWithRecipe: invalid recipe ID!");

            Guid currentHotel = await GetCurrentHotelId();
            var recipe = await GetRecipeWithArticleInstances(currentHotel, id);
            
            if (recipe == null)
                return BadRequest("CookWithRecipe: there's no recipe with given ID!");

            ViewBag.ResultMessage = string.Empty;

            return View(recipe);
        }


        [HttpPost]
        [Authorize(Roles="KitchenEmployee")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CookWithRecipeTakeArticle(CookWithRecipeTakeArticleViewModel model)
        {
            if (model.RecipeId == Guid.Empty)
                return BadRequest("CookWithRecipeTakeArticle: invalid recipe ID!");


            var artInstance = await _context.KitchenArticleInstances
                .Where(ai => ai.Id == model.ArticleInstanceId)
                .FirstOrDefaultAsync();

            if (artInstance == null)
                return BadRequest("CookWithRecipeTakeArticle: there's no article instance with given ID!");


            KitchenApiController apiController = new KitchenApiController(_context)
            {
                ControllerContext = this.ControllerContext
            };
            if (await apiController.TakeArticleInstances(model.ArticleInstanceId, model.Count))
                ViewBag.ResultMessage = "Pomyślnie zabrano artykuł z miejsca przechowywania!";
            else
                ViewBag.ResultMessage = "Za mała ilość artykułu w tym miejscu!";


            Guid currentHotel = await GetCurrentHotelId();
            var recipe = await GetRecipeWithArticleInstances(currentHotel, model.RecipeId);
            
            if (recipe == null)
                return BadRequest("CookWithRecipeTakeArticle: there's no recipe with given ID!");
            
            return View("CookWithRecipe", recipe);
        }


        [HttpGet]
        [Authorize(Roles="KitchenEmployee")]
        public async Task<IActionResult> SpecialDishInspiration()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles="KitchenEmployee")]
        public async Task<IActionResult> SearchForDishCategory()
        {
            var categories = await _recipeService.GetCategories();

            return View(categories);
        }


        [HttpGet]
        [Authorize(Roles="KitchenEmployee")]
        public async Task<IActionResult> SearchForDishArea()
        {
            var areas = await _recipeService.GetAreas();

            return View(areas);
        }


        [HttpGet]
        [Authorize(Roles="KitchenEmployee")]
        public async Task<IActionResult> SearchForDishByName()
        {
            var meals = new List<ExternalMeal>();

            ViewBag.PrevSearch = "";
            return View(meals);
        }


        [HttpGet]
        [Authorize(Roles="KitchenEmployee")]
        public async Task<IActionResult> RandomSpecialDish()
        {
            var meal = await _recipeService.GetRandomMeal();
            if (meal == null)
                return BadRequest("Could not retrieve a random meal!");

            return View("DisplaySpecialDish", meal);
        }


        [HttpGet]
        [Authorize(Roles="KitchenEmployee")]
        public async Task<IActionResult> SpecialDishesOfCategory(string category)
        {
            var dishes = await _recipeService.GetMealsOfCategory(category);

            return View("FilterSpecialDishes", dishes);
        }


        [HttpGet]
        [Authorize(Roles="KitchenEmployee")]
        public async Task<IActionResult> SpecialDishesOfArea(string area)
        {
            var dishes = await _recipeService.GetMealsFromArea(area);

            return View("FilterSpecialDishes", dishes);
        }


        [HttpPost]
        [Authorize(Roles="KitchenEmployee")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SearchForDish(string name)
        {
            var meals = await _recipeService.SearchMealsByName(name);

            ViewBag.PrevSearch = name;
            return View("SearchForDishByName", meals);
        }

        
        [HttpGet]
        [Authorize(Roles="KitchenEmployee")]
        public async Task<IActionResult> DisplaySpecialDish(string id)
        {
            var meal = await _recipeService.GetMealDetails(id);
            if (meal == null)
                return BadRequest("Invalid meal ID!");

            return View(meal);
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


        private async Task<KitchenRecipe?> GetRecipeWithArticleInstances(Guid hotelId, Guid recipeId)
        {
            var recipe = await _context.KitchenRecipes
                .AsNoTracking()
                .Where(kr => kr.Id == recipeId)
                .Include(kr => kr.OutcomeProduct)
                .Include(kr => kr.Ingredients)
                    !.ThenInclude(kri => kri.Article)
                    .ThenInclude(ka => ka!.Instances)
                    !.ThenInclude(ai => ai.Storage)
                .FirstOrDefaultAsync();
            
            if (recipe == null)
                return null;

            ViewBag.ResultMessage = string.Empty;

            // Pozbywamy się artykułów przechowywanych poza hotelem

            foreach (var ing in recipe.Ingredients!)
            {
                for (int i = 0; i < ing.Article!.Instances!.Count;)
                {
                    if (ing.Article.Instances[i].Storage!.HotelId != hotelId)
                        ing.Article.Instances.RemoveAt(i);
                    else
                        i++;
                }
            }

            return recipe;
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