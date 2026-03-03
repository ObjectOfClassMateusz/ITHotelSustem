namespace HotelSystemIndustry.Models
{
    public class Phone
    {
        public Guid Id { get; set; }
        public Guid HotelId { get; set; }
        public Hotel Hotel { get; set; }
        public string PhoneNumber { get; set; }
    }
}
