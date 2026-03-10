using System.ComponentModel.DataAnnotations;

namespace HotelSystemIndustry.Models
{
    public class Phone
    {
        [Key]
        public Guid Id { get; set; }
        [Required, Phone]
        public required string PhoneNumber { get; set; }

        public Guid HotelId { get; set; }
        public Hotel? Hotel { get; set; }
    }
}
