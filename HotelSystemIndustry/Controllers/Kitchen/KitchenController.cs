using Microsoft.AspNetCore.Mvc;

namespace HotelSystemIndustry.Controllers.Kitchen
{
    
    public class KitchenController : Controller
    {
        
        public KitchenController() {}

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View();
        }

    }

}