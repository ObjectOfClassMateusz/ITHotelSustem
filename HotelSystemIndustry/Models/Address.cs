using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace HotelSystemIndustry.Models
{
    [PrimaryKey(nameof(Street), nameof(City), nameof(PostalCode))]
    public class Address
    {
        [Required]
        public string Street { get; set; } = string.Empty;//PK
        [Required]
        public string City { get; set; } = string.Empty;//PK
        [Required]
        public string PostalCode { get; set; } = string.Empty;//PK
        [Required]
        public string Country { get; set; } = string.Empty;

        public Guid HotelId { get; set; }
        public Hotel? Hotel { get; set; }
    }
}
