using System.ComponentModel.DataAnnotations;

namespace HotelSystemIndustry.Models.Trading
{
    
    public class ShopPoint
    {
        [Key]
        public Guid Id { get; set; }

        [Required, MaxLength(50, ErrorMessage = "Opis lokalizacji jest zbyt długi")]
        public virtual required string Location { get; set; }

        public Purchase? Purchase { get; set; }
    }

}