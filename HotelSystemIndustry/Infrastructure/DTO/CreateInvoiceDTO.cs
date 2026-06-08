using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace HotelSystemIndustry.Infrastructure.DTO
{
    public class CreateInvoiceDTO
    {
        public Guid HotelId { get; set; }
        public string HotelName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Wybierz rezerwację")]
        public Guid ReservationId { get; set; }
        public List<SelectListItem> AvailableReservations { get; set; } = new();

        [Required(ErrorMessage = "Wpisz numer faktury")]
        public string InvoiceNumber { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        public DateTime IssueDate { get; set; } = DateTime.Today;

        [Range(0, double.MaxValue, ErrorMessage = "Kwota musi być dodatnia")]
        public decimal TotalAmount { get; set; }
    }
}
