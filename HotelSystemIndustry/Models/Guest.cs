using System.ComponentModel.DataAnnotations;

namespace HotelSystemIndustry.Models
{
    public class Guest
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        public string LastName { get; set; } = string.Empty;
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;

        public Guid HotelId { get; set; }
        public Hotel? Hotel { get; set; }

        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();


        public ICollection<Recreation.RecreationBooking> RecreationBookings { get; set; } = new List<Recreation.RecreationBooking>();
    }
}
