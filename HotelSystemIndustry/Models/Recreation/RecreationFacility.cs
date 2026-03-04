using System.ComponentModel.DataAnnotations;

namespace HotelSystemIndustry.Models.Recreation
{
    public class RecreationFacility
    {
        [Key]
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        [Required]
        public uint MaxCapacity { get; set; }
        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal PricePerHour { get; set; }

        public ICollection<RecreationBooking> Bookings { get; set; } = new List<RecreationBooking>();
    }
}