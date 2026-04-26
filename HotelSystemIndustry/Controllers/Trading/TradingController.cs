using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelSystemIndustry.Controllers.Trading
{
    [Authorize(Roles="TradingEmployee")]
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