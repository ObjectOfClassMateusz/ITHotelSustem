using HotelSystemIndustry.Infrastructure;
using HotelSystemIndustry.Infrastructure.DTO;
using HotelSystemIndustry.Models;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text.RegularExpressions;

namespace HotelSystemIndustry.Controllers
{
    //[Authorize(Roles="KitchenEmployee,MaintenanceEmployee,Admin")]
    public class HotelsController : Controller
    {
        private readonly HotelDbContext _context;
        public HotelsController(HotelDbContext context){
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Hotels.ToListAsync());
        }

        [HttpGet]
        public IActionResult Privacy()
        {
            return View("Privacy");
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
                return NotFound();

            var hotel = await _context.Hotels
                .Include(h => h.Address)
                .Include(h => h.PhoneNumbers)
                .Include(h => h.Rooms)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (hotel == null)
                return NotFound();

            return View(hotel);
        }
        //inne dla innych kont


        //
        [HttpGet]
        [Authorize(Roles = "HotelEmployee")]
        public async Task<IActionResult> Calendar(Guid? id, int? day, int? month, int? year)
        {
            if (id == null) return NotFound();

            var today = DateTime.Today;
            int y = year ?? today.Year;
            int m = month ?? today.Month;
            int d = day ?? today.Day;

            // Zabezpieczenie przed nieprawidłowymi wartościami
            if (m < 1 || m > 12) m = today.Month;
            if (y < 2000 || y > 2100) y = today.Year;

            var hotel = await _context.Hotels
                .Include(h => h.Rooms)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (hotel == null) return NotFound();

            // Zakres kalendarza: 30 dni od wybranej daty
            var startDate = new DateTime(y, m, d);
            var endDate = startDate.AddDays(30);

            // Rezerwacje w zakresie widoku
            /*var reservations = await _context.Reservations
                .Include(r => r.Room)
                .Where(r => r.Room.HotelId == id
                         && r.CheckOutDate > startDate
                         && r.CheckInDate < endDate)
                .ToListAsync();*/

            ViewBag.Hotel = hotel;
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;
            //ViewBag.Reservations = reservations;
            ViewBag.Today = today;

            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View(new CreateHotelDTO());
        }

        private static class ValidatePhone
        {
            private static readonly Regex PhoneRegex =
                new Regex(@"^\+\d{2}\s\d{3}\s\d{3}\s\d{3}$");

            public static bool IsValid(IEnumerable<string> phones)
            {
                return phones != null &&
                       phones.All(t => PhoneRegex.IsMatch(t));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CreateHotelDTO hotelDTO)
        {
            bool telefonyOk = ValidatePhone.IsValid(hotelDTO.PhoneNumbers);
            if (!telefonyOk)
            {
                ViewBag.ErrorPhones = "Podane telefony są puste bądź nieprawidłowe";
                return View();
            }
            Hotel hotel = new Hotel();
            Guid newID = Guid.NewGuid();
            hotel.Id = newID;
            hotel.Name = hotelDTO.Name;
            hotel.Description = hotelDTO.Description;
            hotel.Email = hotelDTO.Email;
            Address address = new Address();
            address.Street = hotelDTO.Street;
            address.City = hotelDTO.City;
            address.PostalCode = hotelDTO.PostalCode;
            address.Country = hotelDTO.Country;
            address.Hotel = hotel;
            address.HotelId = newID;
            hotel.Address = address;

            foreach (var phone in hotelDTO.PhoneNumbers)
            {
                Phone p = new Phone()
                { 
                    PhoneNumber = phone.ToString()
                };
                hotel.PhoneNumbers.Add(p);
                _context.Add(p);
            }
            Console.WriteLine(hotel.PhoneNumbers.Count);

            _context.Add(hotel);
            _context.Add(address);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Hotels/Edit/5
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return NotFound();

            var hotel = await _context.Hotels
                .Include(h => h.Address)
                .Include(h => h.PhoneNumbers)
                .FirstOrDefaultAsync(h => h.Id == id);
            if (hotel == null) return NotFound();
            return View(hotel);
        }

        // POST: Hotels/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Hotel model,List<Phone> PhoneNumbers)
        {
            
            var hotel = await _context.Hotels.FindAsync(id)
                ?? throw new Exception("Nie znaleziono Hotelu do Usunięcia!");
            var address = await _context.Addresses.FirstOrDefaultAsync(a => a.HotelId == id)
                ?? throw new Exception("Nie znaleziono Adresu do Usunięcia");
            List<Phone> phones = await _context.Phones
                .Where(p => p.HotelId == id)
                .ToListAsync();

            address.City = model.Address.City;
            address.Country = model.Address.Country;
            address.PostalCode = model.Address.PostalCode;
            address.Street = model.Address.Street;
            _context.Addresses.Update(address);

            hotel.Name = model.Name;
            hotel.Description = model.Description;
            hotel.Email = model.Email;

            
            _context.Hotels.Update(hotel);
            _context.Phones.RemoveRange(hotel.PhoneNumbers);

            var newPhones = PhoneNumbers
                .Where(p => !string.IsNullOrWhiteSpace(p.PhoneNumber))
                .Select(p => new Phone
                {
                    Id = Guid.NewGuid(),
                    PhoneNumber = p.PhoneNumber,
                    HotelId = hotel.Id
                }).ToList();

            await _context.Phones.AddRangeAsync(newPhones);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = hotel.Id });
        }

        // GET: Hotels/Delete/id
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
                return NotFound();
            var hotel = await _context.Hotels
                .FirstOrDefaultAsync(m => m.Id == id);
            if (hotel == null)
                return NotFound();
            return View(hotel);
        }

        // POST: Hotels/Delete/
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var hotel = await _context.Hotels.FindAsync(id)
                ?? throw new Exception("Nie znaleziono Hotelu do Usunięcia!");
            var address = await _context.Addresses.FirstOrDefaultAsync(a => a.HotelId == hotel.Id)
                ?? throw new Exception("Nie znaleziono Adresu do Usunięcia");

            List<Phone> phones = await _context.Phones
                .Where(p => p.HotelId == hotel.Id)
                .ToListAsync();

            foreach (var phone in phones)
            {
                _context.Phones.Remove(phone);
            }
            _context.Addresses.Remove(address);
            _context.Hotels.Remove(hotel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Hotels/AddRoom/id
        [HttpGet]
        public async Task<IActionResult> AddRoom(Guid? id)
        {
            if (id == null) 
                return NotFound();
            var hotel = await _context.Hotels.FindAsync(id);
            if (hotel == null) 
                return NotFound();
            var dto = new AddRoomDTO
            {
                HotelId = hotel.Id,
                HotelName = hotel.Name
            };
            return View(dto);
        }

        // POST: Hotels/AddRoom
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRoom(AddRoomDTO dto)
        {
            if (!ModelState.IsValid) 
                return View(dto);

            var hotel = await _context.Hotels.FindAsync(dto.HotelId);
            if (hotel == null)
                return NotFound();

            bool duplicateNumber = await _context.Rooms
                .AnyAsync(r => r.HotelId == dto.HotelId && r.RoomNumber == dto.RoomNumber);

            if (duplicateNumber)
            {
                ModelState.AddModelError(nameof(dto.RoomNumber),
                    $"Pokój nr {dto.RoomNumber} już istnieje w tym hotelu.");
                return View(dto);
            }

            decimal convertedValue = decimal.Parse(dto.BasePricePerNight.Replace(',', '.'),
                CultureInfo.InvariantCulture);

            var room = new Room
            {
                Id = Guid.NewGuid(),
                RoomNumber = dto.RoomNumber,
                Floor = dto.Floor,
                Capacity = dto.Capacity,
                BasePricePerNight = convertedValue,
                Renovation = dto.Renovation,
                RoomType = dto.RoomType,
                HotelId = dto.HotelId
            };
            await _context.Rooms.AddAsync(room);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = dto.HotelId });
        }

        private bool HotelExists(Guid id) 
            =>_context.Hotels.Any(e => e.Id == id);
    }
}