using System.Collections.ObjectModel;
using HotelSystemIndustry.Infrastructure;
using HotelSystemIndustry.Models;
using HotelSystemIndustry.Models.Events;
using HotelSystemIndustry.Models.Kitchen;
using HotelSystemIndustry.ViewModels.Events;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HotelSystemIndustry.Controllers.Events
{
    
    public class EventManagementController : Controller
    {
        
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



            var result = await SaveReservationInDatabase(model);

            if (result == ReservationUploadResult.FileUploadingError)
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



        [HttpGet]
        [Authorize(Roles="HotelEmployee")]
        public async Task<IActionResult> EventChoosing()
        {
            var currentTimeMinusWeek = DateTime.UtcNow.AddDays(-7);

            var eventReservs = await _context.EventReservations
                .AsNoTracking()
                .Where(er => er.EndTime >= currentTimeMinusWeek)
                .Include(er => er.EventType)
                .Include(er => er.Status)
                .ToListAsync();

            ViewBag.EventReservations = eventReservs;
            return View();
        }


        [HttpGet]
        [Authorize(Roles="HotelEmployee")]
        public async Task<IActionResult> EventRealisation([FromRoute] Guid id)
        {
            var reservation = await _context.EventReservations
                .AsNoTracking()
                .Include(r => r.EventType)
                .Include(r => r.Status)
                .Include(r => r.Halls)
                .Include(r => r.Equipment)
                    !.ThenInclude(e => e.Equipment)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reservation == null)
                return BadRequest("Invalid event reservation ID!");

            ViewBag.StatusList = new SelectList(_context.EventReservationStatuses, "Id", "Name", reservation.StatusId);
            return View(reservation);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles="HotelEmployee")]
        public async Task<IActionResult> EventUpdateStatus(Guid id, Guid statusId)
        {
            var reservation = await _context.EventReservations
                .Include(r => r.EventType)
                .Include(r => r.Halls)
                .Include(r => r.Equipment)
                    !.ThenInclude(e => e.Equipment)
                .FirstOrDefaultAsync(r => r.Id == id);

            var status = await _context.EventReservationStatuses
                .FirstOrDefaultAsync(s => s.Id == statusId);

            if (reservation == null)
                return BadRequest("Invalid event reservation ID!");

            if (status == null)
                return BadRequest("Invalid event status ID!");

            reservation.StatusId = statusId;
            reservation.Status = status;
            _context.EventReservations.Update(reservation);
            await _context.SaveChangesAsync();

            ViewBag.StatusList = new SelectList(_context.EventReservationStatuses, "Id", "Name", reservation.StatusId);
            return View("EventRealisation");
        }


        [HttpGet]
        [Authorize(Roles="HotelEmployee")]
        public async Task<IActionResult> GetEventAgreementDocument(Guid id)
        {
            var reservation = await _context.EventReservations
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reservation == null)
                return BadRequest("Invalid event reservation ID!");

            string fullPath = Path.Combine(_appEnvironment.WebRootPath, "EventAgreements", reservation.AgreementDocumentPath);
            
            var stream = new FileStream(fullPath, FileMode.Open);
            return new FileStreamResult(stream, "application/pdf");
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

        private enum ReservationUploadResult
        {
            Success,
            InvalidDataError,
            DatabaseSavingError,
            FileUploadingError
        }

        private async Task<ReservationUploadResult> SaveReservationInDatabase(BookingEventViewModel model)
        {
            var status = await _context.EventReservationStatuses.FirstOrDefaultAsync(s => s.Value == "booked");
            if (status == null)
            {
                status = new EventReservationStatus
                {
                    Id = Guid.NewGuid(), Name = "Booked", Value = "booked", IsActive = true
                };
                _context.Add(status);
            }


            var eventType = await _context.EventTypes.FirstOrDefaultAsync(t => t.Id == model.EventTypeId);
            if (eventType == null)
                return ReservationUploadResult.InvalidDataError;
            

            EventReservation reservation = new EventReservation
            {
                Id = Guid.NewGuid(),
                StatusId = status.Id,
                Status = status,
                EventTypeId = eventType.Id,
                EventType = eventType,
                StartTime = model.StartTime.ToUniversalTime(),
                EndTime = model.EndTime.ToUniversalTime(),
                NumRequiredStaff = model.NumServantStuff,
                NumGuests = model.NumGuests,
                Halls = new Collection<EventHall>(),
                Equipment = new Collection<EquipmentInstance>()
            };

            foreach (var hallReservation in model.Halls)
            {
                if (!hallReservation.Selected)
                    continue;

                EventHall? eventHall = await _context.EventHalls
                    .Include(eh => eh.Equipment)
                    .FirstOrDefaultAsync(h => h.Id == hallReservation.HallId);
                if (eventHall == null)
                    return ReservationUploadResult.InvalidDataError;

                reservation.Halls.Add(eventHall);

                foreach (var equipmentReservation in hallReservation.Equipment)
                {
                    if (!equipmentReservation.Selected)
                        continue;

                    var equipmentInstance = eventHall.Equipment!.FirstOrDefault(e => e.Id == equipmentReservation.EquipmentInstanceId);
                    if (equipmentInstance == null)
                        return ReservationUploadResult.InvalidDataError;

                    reservation.Equipment.Add(equipmentInstance);
                }
            }

            var savedPath = await SaveAgreementFile(model);

            if (string.IsNullOrEmpty(savedPath))
                return ReservationUploadResult.FileUploadingError;

            reservation.AgreementDocumentPath = Path.GetFileName(savedPath);

            _context.EventReservations.Add(reservation);
            await _context.SaveChangesAsync();

            return ReservationUploadResult.Success;
        }

    }

}