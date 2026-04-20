using Microsoft.AspNetCore.Mvc;

namespace HotelSystemIndustry.Controllers.Reception
{
    
    public class ReceptionController : Controller
    {
        
        public ReceptionController() {}


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View();
        }

    }

}