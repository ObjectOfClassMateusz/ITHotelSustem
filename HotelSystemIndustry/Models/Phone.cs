using System.ComponentModel.DataAnnotations;

namespace HotelSystemIndustry.Models
{
    public class Phone
    {
        [Key]
        public Guid Id { get; set; }
        public Guid HotelId { get; set; }
        public Hotel Hotel { get; set; }
        [Phone]
        public string PhoneNumber { get; set; }
    }
}
