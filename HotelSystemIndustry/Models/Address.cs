namespace HotelSystemIndustry.Models
{
    public class Address
    {
        public string Street { get; set; } = string.Empty;//PK
        public string City { get; set; } = string.Empty;//PK
        public string PostalCode { get; set; } = string.Empty;//PK
        public string Country { get; set; } = string.Empty;
    }
}
