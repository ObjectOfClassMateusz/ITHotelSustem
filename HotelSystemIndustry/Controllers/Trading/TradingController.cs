using Microsoft.AspNetCore.Mvc;

namespace HotelSystemIndustry.Controllers.Trading
{
    
    public class TradingController : Controller
    {
        
        public TradingController() {}


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View();
        }

    }

}