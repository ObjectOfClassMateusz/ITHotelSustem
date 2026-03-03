namespace HotelSystemIndustry.Models
{
    public class Hotel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        
        public Address Address { get; set; } = null!;

        public IEnumerable<Phone> PhoneNumbers { get; set; } = Enumerable.Empty<Phone>();
        public ICollection<Room> Rooms { get; set; } = new List<Room>();
    }
}
