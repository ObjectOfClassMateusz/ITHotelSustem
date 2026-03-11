namespace HotelSystemIndustry.Models
{
    public class RaportPayment
    {
        public Guid RaportId { get; set; }
        public Raport Raport { get; set; }

        public Guid PaymentId { get; set; }
        public Payment Payment { get; set; }
    }
}
