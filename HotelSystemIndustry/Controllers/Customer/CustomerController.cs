using Microsoft.AspNetCore.Mvc;

namespace HotelSystemIndustry.Controllers.Customer
{
    
    public class CustomerController : Controller
    {
        
        public CustomerController() {}


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View();
        }

    }

}