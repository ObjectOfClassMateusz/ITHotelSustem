namespace HotelSystemIndustry.Models
{
    public enum ReservationStatus
    {
        Pending = 0,
        Confirmed = 1,
        Cancelled = 2,
        Completed = 3
    }

    public class Reservation
    {
        public Guid Id { get; set; }

        public Guid RoomId { get; set; }
        public Room Room { get; set; } = null!;

        public ICollection<Guest> Guests { get; set; } = new List<Guest>();

        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }

        public ReservationStatus Status { get; set; }
        public Payment? Payment { get; set; }
        public int NumberOfOvernightStays { get; set; }
        public Address Address { get; set; }
        public string NIP {  get; set; }
        public String SpecialWishes { get; set; }
    }
}
