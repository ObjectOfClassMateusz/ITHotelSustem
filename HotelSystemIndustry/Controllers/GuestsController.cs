using HotelSystemIndustry.Infrastructure;
using HotelSystemIndustry.Infrastructure.DTO;
using HotelSystemIndustry.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;

namespace HotelSystemIndustry.Controllers
{
    //[ApiController]
    //[Route("api/[controller]")]

    //swagger
    public class GuestsController : Controller
    {
        private readonly HotelDbContext _context;
        public GuestsController(HotelDbContext context) => _context = context;


        // GET: Guests/Create?hotelId=...
        [HttpGet]
        public async Task<IActionResult> Create(Guid hotelId)
        {
            var hotel = await _context.Hotels.FindAsync(hotelId);
            if (hotel == null) return NotFound();

            var dto = new CreateGuestDTO
            {
                HotelId = hotel.Id,
                HotelName = hotel.Name
            };
            return View(dto);
        }

        // POST: Guests/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateGuestDTO dto)
        {
            if (!ModelState.IsValid) return View(dto);

            // Sprawdź duplikat e-mail w hotelu
            bool duplicate = await _context.Guests.AnyAsync(g =>
                g.HotelId == dto.HotelId && g.Email == dto.Email
                && !string.IsNullOrEmpty(dto.Email));

            if (duplicate)
            {
                ModelState.AddModelError(nameof(dto.Email),
                    "Gość z tym adresem e-mail już istnieje w tym hotelu.");
                return View(dto);
            }

            var guest = new Guest
            {
                Id = Guid.NewGuid(),
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                HotelId = dto.HotelId
            };

            await _context.Guests.AddAsync(guest);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = guest.Id });
        }

        // GET: Guests/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var guest = await _context.Guests
                .Include(g => g.Hotel)
                .Include(g => g.Reservations)
                    .ThenInclude(r => r.Room)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (guest == null) return NotFound();
            return View(guest);
        }

        // GET: Guests/Index?hotelId=...
        [HttpGet]
        public async Task<IActionResult> Index(Guid hotelId)
        {
            var hotel = await _context.Hotels.FindAsync(hotelId);
            if (hotel == null) return NotFound();

            var guests = await _context.Guests
                .Where(g => g.HotelId == hotelId)
                .OrderBy(g => g.LastName)
                .ToListAsync();

            ViewBag.Hotel = hotel;
            return View(guests);
        }
    }
}
