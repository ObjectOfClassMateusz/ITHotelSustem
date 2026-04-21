using Microsoft.AspNetCore.Mvc;

namespace HotelSystemIndustry.Controllers.Recreation
{
    
    public class RecreationController : Controller
    {
        
        public RecreationController() {}


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View();
        }

    }

}