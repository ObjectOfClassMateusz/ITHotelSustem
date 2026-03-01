namespace HotelSystemIndustry.Models
{
    public enum RoomType
    {
        Single = 0,
        Double = 1,
        Studio= 2,
        Apartment = 3
    }

    public class Room
    {
        public Guid Id { get; set; }
        public string RoomNumber { get; set; } = string.Empty;
        public int Floor { get; set; }
        public int Capacity { get; set; }
        public decimal BasePricePerNight { get; set; }
        public bool Renovation { get; set; }

        public RoomType RoomType { get; set; }

        public Guid HotelId { get; set; }
        public Hotel Hotel { get; set; } = null!;
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}
