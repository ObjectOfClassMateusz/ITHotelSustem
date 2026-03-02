using System.ComponentModel.DataAnnotations;

namespace HotelSystemIndustry.Models.Recreation
{
    public class RecreationFacility
    {
        [Key]
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public int MaxCapacity { get; set; } 
        public decimal PricePerHour { get; set; }

        public ICollection<RecreationBooking> Bookings { get; set; } = new List<RecreationBooking>();
    }
}