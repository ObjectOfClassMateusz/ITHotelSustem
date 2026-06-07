using HotelSystemIndustry.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace HotelSystemIndustry.Infrastructure.DTO
{
    public class CreateReservationDTO
    {
        public Guid HotelId { get; set; }
        public string HotelName { get; set; } = string.Empty;

        // Pokój
        public Guid RoomId { get; set; }
        public List<SelectListItem> AvailableRooms { get; set; } = new();

        // Daty
        [Required(ErrorMessage = "Wybierz datę zameldowania")]
        [DataType(DataType.Date)]
        public DateTime CheckInDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Wybierz datę wymeldowania")]
        [DataType(DataType.Date)]
        public DateTime CheckOutDate { get; set; } = DateTime.Today.AddDays(1);

        // Dane
        [Required(ErrorMessage = "Wpisz NIP")]
        [MaxLength(10)]
        public string NIP { get; set; } = string.Empty;

        public string? SpecialWishes { get; set; }

        public ReservationStatus Status { get; set; } = ReservationStatus.Pending;

        // Goście (z hotelu)
        public List<Guid> SelectedGuestIds { get; set; } = new();
        public List<SelectListItem> AvailableGuests { get; set; } = new();

        // Płatność
        [Required(ErrorMessage = "Wybierz metodę płatności")]
        public PaymentMethod PaymentMethod { get; set; }

        public List<SelectListItem> PaymentMethods { get; set; } = new();
    }
}
