namespace HotelSystemIndustry.Models
{
    public class Invoice
    {
        public Guid Id { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public DateTime IssueDate { get; set; }

        public Guid ReservationId { get; set; }
        public Reservation Reservation { get; set; } = null!;

        public decimal TotalAmount { get; set; }
    }
}
