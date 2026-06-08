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
                ModelState.AddModelError(string.Empty, "Wydarzenie nie może zostać zorganizowane w przeszłości!");
                ViewBag.EventTypes = new SelectList(_context.EventTypes, "Id", "Name");
                return View("StartMakingReservation", model);
            }
            if (model.StartTime >= model.EndTime)
            {
                ModelState.AddModelError(string.Empty, "Wydarzenie nie może się skończyć, zanim się rozpocznie!");
                ViewBag.EventTypes = new SelectList(_context.EventTypes, "Id", "Name");
                return View("StartMakingReservation", model);
            }


            var freeHalls = await GetApiController().GetAvailableEventHalls(model.StartTime,
                                                                            model.EndTime);

            model.Halls.Clear();
            foreach (var hall in freeHalls)
            {
                model.Halls.Add(new BookingEventViewModel.HallSelection{ HallId = hall.Id, Selected = false });
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
            var selectedHallIds = new List<Guid>();
            foreach (var hall in model.Halls)
            {
                if (hall.Selected)
                    selectedHallIds.Add(hall.HallId);
            }

            ViewBag.NumMaxGuests = await GetApiController().CalcMaxGuestCount(selectedHallIds);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyAndRetrieveAgreement(BookingEventViewModel model)
        {
            var selectedHallIds = new List<Guid>();
            foreach (var hall in model.Halls)
            {
                if (hall.Selected)
                    selectedHallIds.Add(hall.HallId);
            }

            uint numMaxGuests = await GetApiController().CalcMaxGuestCount(selectedHallIds);
            if (model.NumGuests > numMaxGuests)
            {
                ModelState.AddModelError("NumGuests", $"Liczba gości nie może przekroczyć {numMaxGuests} przy wybranych halach.");
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
                var halls = await _context.EventHalls
                    .AsNoTracking()
                    .Include(eh => eh.Equipment)
                        !.ThenInclude(ei => ei.Equipment)
                            .ThenInclude(e => e!.Type)
                    .ToListAsync();

                ViewBag.TypeName = (await _context.EventTypes.FirstOrDefaultAsync(t => t.Id == model.EventTypeId))!.Name;
                ViewBag.EventHalls = halls;
                return View("VerifyAndRetrieveAgreement", model);
            }

            var result = await GetApiController().SubmitReservation(model);

            if (result == ReservationUploadResult.Success)
                return RedirectToAction("EventReservationSuccess");
            

            if (result == ReservationUploadResult.FileUploadingError)
                ModelState.AddModelError("AgreementFile", "Wystąpił błąd przy przesyłaniu dokumentu z umową!");
            else
                ModelState.AddModelError(string.Empty, "Wystąpił błąd przy dokonywaniu rezerwacji!");

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

            var eventReservs = await GetApiController().GetEventReservations(currentTimeMinusWeek);

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

            ViewBag.StatusList = new SelectList(_context.EventReservationStatuses, "Id", "Name", statusId);
            return View("EventRealisation", reservation);
        }


        [HttpGet]
        [Authorize(Roles="HotelEmployee")]
        public async Task<IActionResult> GetEventAgreementDocument(Guid id)
        {
            return RedirectToAction("GetEventAgreementDocument", "EventManagementApi", new {Id = id});
        }


        [HttpGet]
        [Authorize(Roles="HotelEmployee")]
        public async Task<IActionResult> CancelEventView(Guid id)
        {
            var reservation = await _context.EventReservations
                .Include(r => r.EventType)
                .Include(r => r.Status)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reservation == null)
                return BadRequest("Invalid event reservation ID!");

            return View(reservation);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles="HotelEmployee")]
        public async Task<IActionResult> ConfirmEventCancelation(Guid id)
        {
            var result = await GetApiController().CancelEvent(id);
            if (!result)
                return BadRequest("Invalid event reservation ID!");

            return RedirectToAction("EventChoosing");
        }

        
        private EventManagementApiController GetApiController()
        {
            EventManagementApiController apiController = new EventManagementApiController(_context, _appEnvironment)
            {
                ControllerContext = this.ControllerContext
            };
            return apiController;
        }

    }

}