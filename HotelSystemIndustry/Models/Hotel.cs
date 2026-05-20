using System.ComponentModel.DataAnnotations;

namespace HotelSystemIndustry.Models
{
    public class Hotel
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; } = string.Empty;
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        public Address Address { get; set; } = null!;
        public IList<Phone> PhoneNumbers { get; set; } = new List<Phone>();

        public ICollection<Room> Rooms { get; set; } = new List<Room>();
    }
}
