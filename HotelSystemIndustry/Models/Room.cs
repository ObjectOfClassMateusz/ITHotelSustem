namespace HotelSystemIndustry.Models
{
    public enum RoomType
    {
        Standard = 0,
        Deluxe = 1,
        Suite = 2,
        Apartment = 3
    }

    public class Room
    {
        public Guid Id { get; set; }
        public string RoomNumber { get; set; } = string.Empty;
        public int Floor { get; set; }
        public int Capacity { get; set; }
        public decimal BasePricePerNight { get; set; }

        public RoomType RoomType { get; set; }

        public Guid HotelId { get; set; }
        public Hotel Hotel { get; set; } = null!;

        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}
