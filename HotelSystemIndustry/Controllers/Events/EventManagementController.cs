using HotelSystemIndustry.Infrastructure;
using HotelSystemIndustry.Models.Events;
using HotelSystemIndustry.ViewModels.Events;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HotelSystemIndustry.Controllers.Events
{
    
    public class EventManagementController : Controller
    {
        // customer / hotwl employe
        private HotelDbContext _context;

        private IWebHostEnvironment _appEnvironment;


        public EventManagementController(HotelDbContext context, IWebHostEnvironment appEnvironment)
        {
            _context = context;
            _appEnvironment = appEnvironment;
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> StartMakingReservation()
        {
            ViewBag.EventTypes = new SelectList(_context.EventTypes, "Id", "Name");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DisplayAvailableEventHalls(BookingEventViewModel model)
        {
            model.StartTime = model.StartTime.ToUniversalTime();
            model.EndTime = model.EndTime.ToUniversalTime();


            if (model.StartTime < DateTime.UtcNow || model.EndTime < DateTime.UtcNow)
            {
                ModelState.AddModelError(string.Empty, "Event reservation time can not be in the past!");
                ViewBag.EventTypes = new SelectList(_context.EventTypes, "Id", "Name");
                return View("StartMakingReservation", model);
            }
            if (model.StartTime >= model.EndTime)
            {
                ModelState.AddModelError(string.Empty, "A reservation can not end before it begins!");
                ViewBag.EventTypes = new SelectList(_context.EventTypes, "Id", "Name");
                return View("StartMakingReservation", model);
            }


            var reservedHalls = await _context.EventReservations
                .AsNoTracking()
                .Where(er => er.EndTime >= model.StartTime && er.StartTime <= model.EndTime)
                .Include(er => er.Halls)
                .Select(eh => eh.Halls)
                .ToListAsync();

            var allHalls = await _context.EventHalls
                .AsNoTracking()
                .Include(eh => eh.Equipment)
                    !.ThenInclude(ei => ei.Equipment)
                        .ThenInclude(e => e!.Type)
                .ToListAsync();

            IList<EventHall> freeHalls = new List<EventHall>();

            model.Halls.Clear();

            foreach (var hall in allHalls)
            {
                foreach (var reservationHallList in reservedHalls)
                {
                    if (reservationHallList == null)
                        continue;

                    foreach (var reservedHall in reservationHallList)
                    {
                        if (reservedHall == null)
                            continue;

                        if (reservedHall.Id == hall.Id)
                            goto hall_end;
                    }
                }

                freeHalls.Add(hall);
                model.Halls.Add(new BookingEventViewModel.HallSelection{ HallId = hall.Id, Selected = false });

                hall_end:;
            }

            ViewBag.FreeHalls = freeHalls;
            
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChooseEventHallEquipment(BookingEventViewModel model)
        {
            var eventHalls = await _context.EventHalls
                .AsNoTracking()
                .Include(eh => eh.Equipment)
                    !.ThenInclude(ei => ei.Equipment)
                        .ThenInclude(e => e!.Type)
                .ToListAsync();

            foreach (var hall in model.Halls)
            {
                if (!hall.Selected)
                    continue;

                hall.Equipment = await _context.EquipmentInstances
                    .AsNoTracking()
                    .Where(ei => ei.EventHallId == hall.HallId)
                    .Select(ei => new BookingEventViewModel.EquipmentSelection{ EquipmentInstanceId = ei.Id, Selected = false })
                    .ToListAsync();
            }

            ViewBag.EventHalls = eventHalls;
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FillEventDetails(BookingEventViewModel model)
        {
            ViewBag.NumMaxGuests = await CalcMaxGuestCount(model);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyAndRetrieveAgreement(BookingEventViewModel model)
        {
            uint numMaxGuests = await CalcMaxGuestCount(model);
            if (model.NumGuests > numMaxGuests)
            {
                ModelState.AddModelError("NumGuests", $"Number of guests can't exceed {numMaxGuests} with the chosen halls.");
                return View("FillEventDetails", model);
            }

            var eventHalls = await _context.EventHalls
                .AsNoTracking()
                .Include(eh => eh.Equipment)
                    !.ThenInclude(ei => ei.Equipment)
                        .ThenInclude(e => e!.Type)
                .ToListAsync();

            ViewBag.TypeName = (await _context.EventTypes.FirstOrDefaultAsync(t => t.Id == model.EventTypeId))!.Name;
            ViewBag.EventHalls = eventHalls;
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitReservation(BookingEventViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var eventHalls = await _context.EventHalls
                    .AsNoTracking()
                    .Include(eh => eh.Equipment)
                        !.ThenInclude(ei => ei.Equipment)
                            .ThenInclude(e => e!.Type)
                    .ToListAsync();

                ViewBag.TypeName = (await _context.EventTypes.FirstOrDefaultAsync(t => t.Id == model.EventTypeId))!.Name;
                ViewBag.EventHalls = eventHalls;
                return View("VerifyAndRetrieveAgreement", model);
            }



            // TODO: submitting to database
            var savedPath = await SaveAgreementFile(model);

            if (string.IsNullOrEmpty(savedPath))
            {
                ModelState.AddModelError("AgreementFile", "Error while uploading the agreement document!");

                var eventHalls = await _context.EventHalls
                    .AsNoTracking()
                    .Include(eh => eh.Equipment)
                        !.ThenInclude(ei => ei.Equipment)
                            .ThenInclude(e => e!.Type)
                    .ToListAsync();

                ViewBag.TypeName = (await _context.EventTypes.FirstOrDefaultAsync(t => t.Id == model.EventTypeId))!.Name;
                ViewBag.EventHalls = eventHalls;
                return View("VerifyAndRetrieveAgreement", model);
            }

            return RedirectToAction("EventReservationSuccess");
        }


        [HttpGet]
        public async Task<IActionResult> EventReservationSuccess()
        {
            return View();
        }


        private async Task<uint> CalcMaxGuestCount(BookingEventViewModel model)
        {
            uint numMaxGuests = 0;

            foreach (var hallSelection in model.Halls)
            {
                if (!hallSelection.Selected)
                    continue;

                var eventHall = await _context.EventHalls
                    .AsNoTracking()
                    .FirstOrDefaultAsync(eh => eh.Id == hallSelection.HallId);
                numMaxGuests += eventHall!.NumMaxGuests;
            }

            return numMaxGuests;
        }

        private async Task<string> SaveAgreementFile(BookingEventViewModel model)
        {
            if (model.AgreementFile == null)
                return string.Empty;

            var extension = Path.GetExtension(model.AgreementFile.FileName).ToLowerInvariant();

            if (string.IsNullOrEmpty(extension) || extension != ".pdf")
                return string.Empty;

            var directoryPath = Path.Combine(_appEnvironment.WebRootPath, "EventAgreements");

            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);


            string targetFileName = Path.GetRandomFileName() + ".pdf";
            var filePath = Path.Combine(directoryPath, targetFileName);

            using (var stream = System.IO.File.Create(filePath))
            {
                await model.AgreementFile.CopyToAsync(stream);
            }

            return filePath;
        }

    }

}