using System.ComponentModel.DataAnnotations;

namespace HotelSystemIndustry.Models
{
    public class Address
    {
        [Key,Required]
        public string Street { get; set; } = string.Empty;//PK
        [Key,Required]
        public string City { get; set; } = string.Empty;//PK
        [Key,Required]
        public string PostalCode { get; set; } = string.Empty;//PK
        [Required]
        public string Country { get; set; } = string.Empty;
    }
}
