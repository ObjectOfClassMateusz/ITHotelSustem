using System.ComponentModel.DataAnnotations;

namespace HotelSystemIndustry.Models.Trading
{
    
    public class ShopPoint
    {
        [Key]
        public Guid Id { get; set; }

        public required string Location { get; set; }
    }

}