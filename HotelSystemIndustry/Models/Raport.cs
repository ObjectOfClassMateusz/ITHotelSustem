using System.ComponentModel.DataAnnotations;

namespace HotelSystemIndustry.Models
{
    public class Raport
    {
        [Key]
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Wymagana nazwa raportu")]
        public string Name { get; set; }

        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        public IEnumerable<Dictionary<Guid, Payment>> SetOfActions { get; set; }
    }
}
