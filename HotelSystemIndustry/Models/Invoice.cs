using System.ComponentModel.DataAnnotations;

namespace HotelSystemIndustry.Models
{
    public class Invoice
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string InvoiceNumber { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        public DateTime IssueDate { get; set; }

        [Required]
        public Guid ReservationId { get; set; }
        public Reservation Reservation { get; set; } = null!;

        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal TotalAmount { get; set; }
    }
}
