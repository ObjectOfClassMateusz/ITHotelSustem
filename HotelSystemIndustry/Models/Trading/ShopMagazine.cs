using System.ComponentModel.DataAnnotations;

namespace HotelSystemIndustry.Models.Trading
{
    
    public class ShopMagazine
    {
        [Key]
        public Guid Id { get; set; }

        [Required, MaxLength(50, ErrorMessage = "Opis lokalizacji jest zbyt długi")]
        public required string Location { get; set; }

        public virtual ICollection<SaleItemInstance>? Items { get; set; }
    }

}