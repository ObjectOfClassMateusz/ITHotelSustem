using System.Diagnostics;
using HotelSystemIndustry.Infrastructure;
using HotelSystemIndustry.Models;
using Microsoft.AspNetCore.Mvc;

namespace HotelSystemIndustry.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HotelDbContext _context;

        public HomeController(ILogger<HomeController> logger , HotelDbContext context)
        {
            _context = context;
            _logger = logger;
        }

        // kontroller/action

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
         //   return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}
    }
}
