using System.ComponentModel.DataAnnotations;

namespace HotelSystemIndustry.Models.Trading
{
    
    public class ShopPoint
    {
        [Key]
        public Guid Id { get; set; }

        [Required, MaxLength(50, ErrorMessage = "Opis lokalizacji jest zbyt długi")]
        public virtual required string Location { get; set; }


        [Required]
        public Guid HotelId { get; set; }

        public Hotel? Hotel { get; set; }
    }

}