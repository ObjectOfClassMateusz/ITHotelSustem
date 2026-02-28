using System.Net;

namespace HotelSystemIndustry.Models
{
    public class Hotel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public IEnumerable<string> PhoneNumbers { get; set; } = Enumerable.Empty<string>();
        public Address Address { get; set; } = null!;

        public ICollection<Room> Rooms { get; set; } = new List<Room>();
    }
}
