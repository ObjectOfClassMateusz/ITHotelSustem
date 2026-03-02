namespace HotelSystemIndustry.Models
{
    public class Guest
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

        public ICollection<Recreation.RecreationBooking> RecreationBookings { get; set; } = new List<Recreation.RecreationBooking>();
    }
}
