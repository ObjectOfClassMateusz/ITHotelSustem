using HotelSystemIndustry.Infrastructure;
using HotelSystemIndustry.Infrastructure.DTO;
using HotelSystemIndustry.Models;
using HotelSystemIndustry.Models.ViewModels;
using HotelSystemIndustry.Services;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace HotelSystemIndustry.Controllers
{
    //[Authorize(Roles="KitchenEmployee,MaintenanceEmployee,Admin")]
    public class HotelsController : Controller
    {
        private readonly HotelDbContext _context;
        private readonly PdfService _pdfService;
        public HotelsController(HotelDbContext context, PdfService pdfService)
        {
            _context = context; _pdfService = pdfService;
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
            if (id == null) 
                return NotFound();

            var today = DateTime.Today;
            int y = year ?? today.Year;
            int m = month ?? today.Month;
            int d = day ?? today.Day;

            //Valid date input
            if (m < 1 || m > 12) 
                m = today.Month;
            if (y < 2000 || y > 2100) 
                y = today.Year;

            var hotel = await _context.Hotels
                .Include(h => h.Rooms)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (hotel == null) 
                return NotFound();

            //Calendar range: 30 days from the selected date
            var startDate = new DateTime(y, m, d).ToUniversalTime();
            var endDate = startDate.AddDays(30).ToUniversalTime();

            // Rezerwacje w zakresie widoku
            var reservations = await _context.Reservations
                .AsNoTracking()                    // ← dodaj
                .Include(r => r.Room)
                .Include(r => r.Guests)
                .Where(r => r.RoomId != null      // filtruj po RoomId bezpośrednio
                         && _context.Rooms
                                .Where(rm => rm.HotelId == id)
                                .Select(rm => rm.Id)
                                .Contains(r.RoomId))
                .Where(r => r.CheckOutDate > startDate
                         && r.CheckInDate < endDate)
                .ToListAsync();

            ViewBag.Hotel = hotel;
            ViewBag.StartDate = startDate.ToUniversalTime();
            ViewBag.EndDate = endDate.ToUniversalTime();
            ViewBag.Reservations = reservations;
            ViewBag.Today = today;

            // DEBUG — usuń po naprawie
            foreach (var r in reservations)
            {
                Console.WriteLine($"Res {r.Id}: RoomId={r.RoomId}, Room={r.Room?.Id}, Room.Number={r.Room?.RoomNumber}");
            }

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
                .Include(h => h.Rooms)
                .FirstOrDefaultAsync(h => h.Id == id);
            if (hotel == null)
            {
                return NotFound();
            }
            return View(hotel);
        }

        // POST: Hotels/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRoom(Guid roomId, Guid hotelId)
        {
            var room = await _context.Rooms.FindAsync(roomId);
            if (room != null)
            {
                _context.Rooms.Remove(room);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Edit), new { id = hotelId });
        }

        [HttpGet]
        public async Task<IActionResult> EditRoom(Guid roomId, Guid hotelId)
        {
            var room = await _context.Rooms.FindAsync(roomId);
            if (room == null) 
                return NotFound();

            var hotel = await _context.Hotels.FindAsync(hotelId);
            if (hotel == null) 
                return NotFound();

            var dto = new EditRoomDTO
            {
                RoomId = room.Id,
                HotelId = hotelId,
                HotelName = hotel.Name,
                RoomNumber = room.RoomNumber,
                Floor = room.Floor,
                Capacity = room.Capacity,
                BasePricePerNight = room.BasePricePerNight,
                Renovation = room.Renovation,
                RoomType = room.RoomType
            };
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRoom(EditRoomDTO dto)
        {
            if (!ModelState.IsValid) return View(dto);

            var room = await _context.Rooms.FindAsync(dto.RoomId);
            if (room == null) return NotFound();

            // Sprawdź duplikat numeru (pomijając siebie)
            bool duplicate = await _context.Rooms.AnyAsync(r =>
                r.HotelId == dto.HotelId &&
                r.RoomNumber == dto.RoomNumber &&
                r.Id != dto.RoomId);

            if (duplicate)
            {
                ModelState.AddModelError(nameof(dto.RoomNumber),
                    $"Pokój nr {dto.RoomNumber} już istnieje w tym hotelu.");
                return View(dto);
            }

            room.RoomNumber = dto.RoomNumber;
            room.Floor = dto.Floor;
            room.Capacity = dto.Capacity;
            room.BasePricePerNight = dto.BasePricePerNight;
            room.Renovation = dto.Renovation;
            room.RoomType = dto.RoomType;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Edit), new { id = dto.HotelId });
        }

        [HttpGet]
        [Authorize(Roles = "HotelEmployee")]
        public async Task<IActionResult> CreateReservation(Guid hotelId)
        {
            var hotel = await _context.Hotels
                .Include(h => h.Rooms)
                .FirstOrDefaultAsync(h => h.Id == hotelId);

            if (hotel == null) 
                return NotFound();

            var guests = await _context.Guests
                .Where(g => g.HotelId == hotelId)
                .OrderBy(g => g.LastName)
                .ToListAsync();

            var dto = new CreateReservationDTO
            {
                HotelId = hotelId,
                HotelName = hotel.Name,
                AvailableRooms = hotel.Rooms
                    .Where(r => !r.Renovation)
                    .OrderBy(r => r.RoomNumber)
                    .Select(r => new SelectListItem(
                        $"#{r.RoomNumber} — {r.RoomType} — {r.BasePricePerNight:C}/noc",
                        r.Id.ToString()))
                    .ToList(),
                AvailableGuests = guests
                    .Select(g => new SelectListItem(
                        $"{g.FirstName} {g.LastName} ({g.Email})",
                        g.Id.ToString()))
                    .ToList(),
                PaymentMethods = Enum.GetValues<PaymentMethod>()
                    .Select(p => new SelectListItem(p.ToString(), ((int)p).ToString()))
                    .ToList()
            };

            return View(dto);
        }

        // POST: Hotels/CreateReservation
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "HotelEmployee")]
        public async Task<IActionResult> CreateReservation(CreateReservationDTO dto)
        {
            if (dto.CheckOutDate <= dto.CheckInDate)
                ModelState.AddModelError(nameof(dto.CheckOutDate),
                    "Data wymeldowania musi być późniejsza niż zameldowania.");

            if (!dto.SelectedGuestIds.Any())
                ModelState.AddModelError(nameof(dto.SelectedGuestIds),
                    "Wybierz co najmniej jednego gościa.");

            if (!ModelState.IsValid)
            {
                // Przeładuj listy
                await ReloadReservationDTO(dto);
                return View(dto);
            }

            var room = await _context.Rooms.FindAsync(dto.RoomId);
            if (room == null) return NotFound();

            var guests = await _context.Guests
                .Where(g => dto.SelectedGuestIds.Contains(g.Id))
                .ToListAsync();

            var nights = (dto.CheckOutDate - dto.CheckInDate).Days;

            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                Method = dto.PaymentMethod,
                Amount = room.BasePricePerNight * nights,
                PaymentDate =  DateTime.Now.ToUniversalTime()
            };

            var reservation = new Reservation
            {
                Id = Guid.NewGuid(),
                CheckInDate = dto.CheckInDate.ToUniversalTime(),
                CheckOutDate = dto.CheckOutDate.ToUniversalTime(),
                Status = dto.Status,
                NumberOfOvernightStays = nights,
                NIP = dto.NIP,
                SpecialWishes = dto.SpecialWishes,
                RoomId = dto.RoomId,
                Payment = payment,
                Guests = guests
            };

            await _context.Reservations.AddAsync(reservation);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Calendar),
                new
                {
                    id = dto.HotelId,
                    day = dto.CheckInDate.Day,
                    month = dto.CheckInDate.Month,
                    year = dto.CheckInDate.Year
                });
        }

        // GET: Hotels/CreateInvoice?hotelId=...
        [HttpGet]
        [Authorize(Roles = "HotelEmployee")]
        public async Task<IActionResult> CreateInvoice(Guid hotelId)
        {
            var hotel = await _context.Hotels.FindAsync(hotelId);
            if (hotel == null) 
            { 
                return NotFound(); 
            }
            var reservations = await _context.Reservations
                .Include(r => r.Room)
                .Include(r => r.Guests)
                .Where(r => r.Room.HotelId == hotelId && r.Invoice == null)
                .OrderByDescending(r => r.CheckInDate)
                .ToListAsync();
            var lastNum = await _context.Invoices.CountAsync() + 1;
            var dto = new CreateInvoiceDTO
            {
                HotelId = hotelId,
                HotelName = hotel.Name,
                InvoiceNumber = $"FV/{DateTime.Today.Year}/{lastNum:D4}",
                IssueDate = DateTime.Today,
                AvailableReservations = reservations.Select(r => new SelectListItem(
                    $"#{r.Id.ToString()[..8]} | " +
                    $"{r.CheckInDate:dd.MM.yy}–{r.CheckOutDate:dd.MM.yy} | " +
                    $"Pokój {r.Room?.RoomNumber} | " +
                    $"{string.Join(", ", r.Guests.Select(g => g.LastName))}",
                    r.Id.ToString())).ToList()
            };
            return View(dto);
        }

        // Helpers
        private async Task ReloadReservationDTO(CreateReservationDTO dto)
        {
            var rooms = await _context.Rooms
                .Where(r => r.HotelId == dto.HotelId && !r.Renovation).ToListAsync();
            var guests = await _context.Guests
                .Where(g => g.HotelId == dto.HotelId).ToListAsync();

            dto.AvailableRooms = rooms.Select(r => new SelectListItem(
                $"#{r.RoomNumber} — {r.RoomType} — {r.BasePricePerNight:C}/noc",
                r.Id.ToString())).ToList();
            dto.AvailableGuests = guests.Select(g => new SelectListItem(
                $"{g.FirstName} {g.LastName}", g.Id.ToString())).ToList();
            dto.PaymentMethods = Enum.GetValues<PaymentMethod>()
                .Select(p => new SelectListItem(p.ToString(), ((int)p).ToString())).ToList();
        }

        private async Task ReloadInvoiceDTO(CreateInvoiceDTO dto)
        {
            var reservations = await _context.Reservations
                .Include(r => r.Room).Include(r => r.Guests)
                .Where(r => r.Room.HotelId == dto.HotelId && r.Invoice == null)
                .ToListAsync();
            dto.AvailableReservations = reservations.Select(r => new SelectListItem(
                $"#{r.Id.ToString()[..8]} | {r.CheckInDate:dd.MM.yy}–{r.CheckOutDate:dd.MM.yy}",
                r.Id.ToString())).ToList();
        }

        // POST: Hotels/CreateInvoice — generuje PDF i zapisuje
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "HotelEmployee")]
        public async Task<IActionResult> CreateInvoice(CreateInvoiceDTO dto)
        {
            if (!ModelState.IsValid)
            {
                await ReloadInvoiceDTO(dto);
                return View(dto);
            }
            bool duplicate = await _context.Invoices
                .AnyAsync(i => i.InvoiceNumber == dto.InvoiceNumber);
            if (duplicate)
            {
                ModelState.AddModelError(nameof(dto.InvoiceNumber),
                    "Faktura o tym numerze już istnieje.");
                await ReloadInvoiceDTO(dto);
                return View(dto);
            }
            var invoice = new Invoice
            {
                Id = Guid.NewGuid(),
                InvoiceNumber = dto.InvoiceNumber,
                IssueDate = dto.IssueDate.ToUniversalTime(),
                ReservationId = dto.ReservationId,
                TotalAmount = dto.TotalAmount
            };
            await _context.Invoices.AddAsync(invoice);
            await _context.SaveChangesAsync();
            // Załaduj dane potrzebne do PDF
            var fullInvoice = await _context.Invoices
                .Include(i => i.Reservation)
                    .ThenInclude(r => r.Room)
                .Include(i => i.Reservation)
                    .ThenInclude(r => r.Guests)
                .FirstAsync(i => i.Id == invoice.Id);
            var pdfBytes = _pdfService.GenerateInvoicePdf(fullInvoice);
            return File(pdfBytes,
                        "application/pdf",
                        $"Faktura_{invoice.InvoiceNumber.Replace("/", "-")}.pdf");
        }

        // GET: Hotels/MonthlySummary?hotelId=...&month=5&year=2026
        [HttpGet]
        [Authorize(Roles = "HotelEmployee")]
        public async Task<IActionResult> MonthlySummary(Guid hotelId, int? month, int? year)
        {
            var hotel = await _context.Hotels.FindAsync(hotelId);
            if (hotel == null) return NotFound();

            int m = month ?? DateTime.Today.Month;
            int y = year ?? DateTime.Today.Year;
            if (m < 1 || m > 12) m = DateTime.Today.Month;

            var start = new DateTime(y, m, 1).ToUniversalTime();
            var end = start.AddMonths(1).ToUniversalTime();

            var reservations = await _context.Reservations
                .Include(r => r.Room)
                .Include(r => r.Guests)
                .Where(r => r.Room.HotelId == hotelId
                         && r.CheckInDate.ToUniversalTime() < end
                         && r.CheckOutDate.ToUniversalTime() > start)
                .ToListAsync();

            var rows = reservations.Select(r => new ReservationRowVM
            {
                RoomNumber = r.Room?.RoomNumber ?? "—",
                CheckIn = r.CheckInDate.ToUniversalTime(),
                CheckOut = r.CheckOutDate.ToUniversalTime(),
                Nights = r.NumberOfOvernightStays,
                Revenue = (r.Room?.BasePricePerNight ?? 0) * r.NumberOfOvernightStays,
                GuestNames = string.Join(", ",
                    r.Guests.Select(g => $"{g.FirstName} {g.LastName}"))
            }).ToList();

            var vm = new MonthlySummaryVM
            {
                HotelId = hotelId,
                HotelName = hotel.Name,
                Month = m,
                Year = y,
                MonthName = start.ToString("MMMM", new System.Globalization.CultureInfo("pl-PL")),
                TotalReservations = rows.Count,
                TotalGuests = reservations.SelectMany(r => r.Guests).Select(g => g.Id).Distinct().Count(),
                TotalNights = rows.Sum(r => r.Nights),
                TotalRevenue = rows.Sum(r => r.Revenue),
                AvgRevenuePerNight = rows.Count > 0 ? rows.Average(r => r.Revenue / Math.Max(r.Nights, 1)) : 0,
                AvgStayLength = rows.Count > 0 ? (decimal)rows.Average(r => r.Nights) : 0,
                Reservations = rows
            };

            ViewBag.Month = m;
            ViewBag.Year = y;
            return View(vm);
        }

        // POST: Hotels/MonthlySummary/Download — pobierz PDF
        [HttpPost]
        [Authorize(Roles = "HotelEmployee")]
        public async Task<IActionResult> MonthlySummaryDownload(Guid hotelId, int month, int year)
        {
            // Pobierz te same dane co GET
            var hotel = await _context.Hotels.FindAsync(hotelId);
            if (hotel == null) return NotFound();

            var start = new DateTime(year, month, 1).ToUniversalTime();
            var end = start.AddMonths(1).ToUniversalTime();

            var reservations = await _context.Reservations
                .Include(r => r.Room).Include(r => r.Guests)
                .Where(r => r.Room.HotelId == hotelId
                         && r.CheckInDate.ToUniversalTime() < end
                         && r.CheckOutDate.ToUniversalTime() > start)
                .ToListAsync();

            var rows = reservations.Select(r => new ReservationRowVM
            {
                RoomNumber = r.Room?.RoomNumber ?? "—",
                CheckIn = r.CheckInDate.ToUniversalTime(),
                CheckOut = r.CheckOutDate.ToUniversalTime(),
                Nights = r.NumberOfOvernightStays,
                Revenue = (r.Room?.BasePricePerNight ?? 0) * r.NumberOfOvernightStays,
                GuestNames = string.Join(", ", r.Guests.Select(g => $"{g.FirstName} {g.LastName}"))
            }).ToList();

            var vm = new MonthlySummaryVM
            {
                HotelId = hotelId,
                HotelName = hotel.Name,
                Month = month,
                Year = year,
                MonthName = start.ToString("MMMM", new System.Globalization.CultureInfo("pl-PL")),
                TotalReservations = rows.Count,
                TotalGuests = reservations.SelectMany(r => r.Guests).Select(g => g.Id).Distinct().Count(),
                TotalNights = rows.Sum(r => r.Nights),
                TotalRevenue = rows.Sum(r => r.Revenue),
                AvgRevenuePerNight = rows.Count > 0 ? rows.Average(r => r.Revenue / Math.Max(r.Nights, 1)) : 0,
                AvgStayLength = rows.Count > 0 ? (decimal)rows.Average(r => r.Nights) : 0,
                Reservations = rows
            };
            var pdfBytes = _pdfService.GenerateMonthlySummaryPdf(vm);
            string name = $"Sprawozdanie_{vm.HotelName}_{vm.MonthName}_{year}.pdf".Replace(" ", "_");
            return File(pdfBytes, "application/pdf", name);
        }

        // GET: Hotels/CancelReservation/{id} — widok potwierdzenia
        [HttpGet]
        [Authorize(Roles = "HotelEmployee")]
        public async Task<IActionResult> CancelReservation(Guid id, Guid hotelId)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Room)
                .Include(r => r.Guests)
                .Include(r => r.Payment)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reservation == null)
            {
                return NotFound();
            }
            ViewBag.HotelId = hotelId;
            return View(reservation);
        }

        // POST: Hotels/CancelReservation
        [HttpPost, ActionName("CancelReservation")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "HotelEmployee")]
        public async Task<IActionResult> CancelReservationConfirmed(Guid id, Guid hotelId)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Payment)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reservation == null) 
            {
                return NotFound();
            } 
            if (reservation.Payment != null)
            {
                _context.Payments.Remove(reservation.Payment);
            }
                
            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Calendar), new { id = hotelId });
        }
    }
}