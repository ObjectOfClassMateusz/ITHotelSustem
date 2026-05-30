using System.Collections.ObjectModel;
using HotelSystemIndustry.Infrastructure;
using HotelSystemIndustry.Models.Events;
using HotelSystemIndustry.ViewModels.Events;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelSystemIndustry.Controllers.Events
{

    public enum ReservationUploadResult
    {
        Success,
        InvalidDataError,
        DatabaseSavingError,
        FileUploadingError
    }


    [ApiController]
    [Route("api/[controller]")]
    public class EventManagementApiController : Controller
    {
        
        private HotelDbContext _context;

        private IWebHostEnvironment _appEnvironment;


        public EventManagementApiController(HotelDbContext context, IWebHostEnvironment appEnvironment)
        {
            _context = context;
            _appEnvironment = appEnvironment;
        }


        [HttpGet("[action]")]
        public async Task<IList<EventType>> GetEventTypes()
        {
            var types = await _context.EventTypes
                .AsNoTracking()
                .ToListAsync();
            return types;
        }


        [HttpGet("[action]")]
        public async Task<IList<EventReservationStatus>> GetEventReservationStatuses()
        {
            var statuses = await _context.EventReservationStatuses
                .AsNoTracking()
                .ToListAsync();
            return statuses;
        }


        [HttpGet("[action]")]
        public async Task<IList<EventHall>> GetAvailableEventHalls(DateTime startTime, DateTime endTime)
        {
            startTime = startTime.ToUniversalTime();
            endTime = endTime.ToUniversalTime();


            if (startTime < DateTime.UtcNow || endTime < DateTime.UtcNow)
            {
                return new List<EventHall>();
            }
            if (startTime >= endTime)
            {
                return new List<EventHall>();
            }


            var reservedHalls = await _context.EventReservations
                .AsNoTracking()
                .Where(er => er.EndTime >= startTime && er.StartTime <= endTime)
                .Include(er => er.Halls)
                .Select(eh => eh.Halls)
                .ToListAsync();

            var allHalls = await _context.EventHalls
                .AsNoTracking()
                .Include(eh => eh.Equipment)
                    !.ThenInclude(ei => ei.Equipment)
                        .ThenInclude(e => e!.Type)
                .Include(eh => eh.Hotel)
                .ToListAsync();

            IList<EventHall> freeHalls = new List<EventHall>();

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

                hall_end:;
            }

            return freeHalls;
        }


        [HttpPost("[action]")]
        public async Task<uint> CalcMaxGuestCount(IList<Guid> selectedHalls)
        {
            uint numMaxGuests = 0;

            foreach (var hallId in selectedHalls)
            {
                var eventHall = await _context.EventHalls
                    .AsNoTracking()
                    .FirstOrDefaultAsync(eh => eh.Id == hallId);

                if (eventHall == null)
                    continue;
                numMaxGuests += eventHall.NumMaxGuests;
            }

            return numMaxGuests;
        }


        [HttpPost("[action]")]
        public async Task<ReservationUploadResult> SubmitReservation(BookingEventViewModel model)
        {
            // Dodatkowa walidacja
            if (model.StartTime.ToUniversalTime() < DateTime.UtcNow ||
                model.EndTime.ToUniversalTime() < DateTime.UtcNow ||
                model.StartTime >= model.EndTime)
                return ReservationUploadResult.InvalidDataError;
            
            var freeHalls = await GetAvailableEventHalls(model.StartTime, model.EndTime);

            var selectedHallIds = new List<Guid>();
            foreach (var hall in model.Halls)
            {
                if (hall.Selected)
                {
                    if (!freeHalls.Any(h => h.Id == hall.HallId))
                        return ReservationUploadResult.InvalidDataError;

                    selectedHallIds.Add(hall.HallId);
                }
            }
            uint maxGuestCount = await CalcMaxGuestCount(selectedHallIds);

            if (model.NumGuests > maxGuestCount)
                return ReservationUploadResult.InvalidDataError;

            

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
                Name = model.Name,
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


        [HttpGet("[action]")]
        [Authorize(Roles="HotelEmployee")]
        public async Task<IList<EventReservation>> GetEventReservations(DateTime startTime)
        {
            var eventReservs = await _context.EventReservations
                .AsNoTracking()
                .Where(er => er.EndTime >= startTime)
                .Include(er => er.EventType)
                .Include(er => er.Status)
                .ToListAsync();

            return eventReservs;
        }


        [HttpPost("[action]")]
        [Authorize(Roles="HotelEmployee")]
        public async Task<bool> EventUpdateStatus(Guid id, Guid statusId)
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
                return false;

            if (status == null)
                return false;

            reservation.StatusId = statusId;
            reservation.Status = status;
            _context.EventReservations.Update(reservation);
            await _context.SaveChangesAsync();
            return true;
        }


        [HttpGet("[action]/{id}")]
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


        [HttpPost("[action]")]
        [Authorize(Roles="HotelEmployee")]
        public async Task<bool> CancelEvent(Guid id)
        {
            var reservation = await _context.EventReservations
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reservation == null)
                return false;

            string agreementDocPath = Path.Combine(_appEnvironment.WebRootPath, "EventAgreements", reservation.AgreementDocumentPath);
            if (System.IO.File.Exists(agreementDocPath))
            {
                System.IO.File.Delete(agreementDocPath);
            }

            _context.EventReservations.Remove(reservation);
            await _context.SaveChangesAsync();

            return true;
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