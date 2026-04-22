using Microsoft.AspNetCore.Mvc;

namespace HotelSystemIndustry.Controllers.HotelManagement
{
    
    public class HotelManagementController : Controller
    {
        
        public HotelManagementController() {}


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View();
        }

    }

}