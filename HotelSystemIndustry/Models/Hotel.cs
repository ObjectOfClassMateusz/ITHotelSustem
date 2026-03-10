using System.ComponentModel.DataAnnotations;

namespace HotelSystemIndustry.Models
{
    public class Hotel
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; } = string.Empty;
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public Address Address { get; set; } = null!;
        public IEnumerable<Phone> PhoneNumbers { get; set; } = Enumerable.Empty<Phone>();
        public ICollection<Room> Rooms { get; set; } = new List<Room>();
    }
}
