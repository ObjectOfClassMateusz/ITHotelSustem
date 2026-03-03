using System.ComponentModel.DataAnnotations;

namespace HotelSystemIndustry.Models.Recreation
{
    public enum BookingStatus
    {
        SCHEDULED,
        IN_PROGRESS,
        COMPLETED,
        CANCELLED
    }

    public class RecreationBooking
    {
        [Key]
        public Guid Id { get; set; }
        public required Guest Guest { get; set; }
        public required RecreationFacility Facility { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public BookingStatus Status { get; set; }
    }
}