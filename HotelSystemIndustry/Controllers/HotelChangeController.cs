using HotelSystemIndustry.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelSystemIndustry.Controllers
{
    
    public class HotelChangeController : Controller
    {

        private HotelDbContext _context;
        
        public HotelChangeController(HotelDbContext context)
        {
            _context = context;
        }


        [HttpPost]
        public async Task<IActionResult> ChangeHotel([FromForm] Guid hotelId)
        {
            HttpContext.Session.SetString("CurrentHotelId", hotelId.ToString());
            return Ok();
        }

        [HttpGet]
        public async Task<Guid> GetCurrentHotel()
        {
            string? currentHotelString = HttpContext.Session.GetString("CurrentHotelId");

            Guid currentHotel = Guid.Empty;
            if (currentHotelString != null)
            {
                currentHotel = Guid.Parse(currentHotelString);
            }
            else
            {
                var hotel = await _context.Hotels.FirstOrDefaultAsync();
                if (hotel != null)
                    currentHotel = hotel.Id;

                await ChangeHotel(currentHotel);
            }
            
            return currentHotel;
        }

    }

}