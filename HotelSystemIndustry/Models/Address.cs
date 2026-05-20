using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace HotelSystemIndustry.Models
{
    public class Address
    {
        [Key]
        public Guid Id { get; set; }
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;

        public Guid HotelId { get; set; }
        public Hotel? Hotel { get; set; }
    }
}
