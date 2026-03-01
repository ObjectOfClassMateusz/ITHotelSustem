namespace HotelSystemIndustry.Models
{
    public class Raport
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public IEnumerable<Dictionary<Guid, Payment>> SetOfActions { get; set; }

    }
}
