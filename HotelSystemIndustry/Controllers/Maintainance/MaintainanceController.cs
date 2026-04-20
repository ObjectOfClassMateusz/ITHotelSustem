using Microsoft.AspNetCore.Mvc;

namespace HotelSystemIndustry.Controllers.Maintainance
{
    
    public class MaintainanceController : Controller
    {
        
        public MaintainanceController() {}


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View();
        }

    }

}