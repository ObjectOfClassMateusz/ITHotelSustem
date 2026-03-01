using System.ComponentModel.DataAnnotations;

namespace HotelSystemIndustry.Models.Trading
{
    
    public class ShopMagazine
    {
        [Key]
        public Guid Id { get; set; }

        public required string Location { get; set; }

        public ICollection<SaleItemInstance>? Items { get; set; }
    }

}