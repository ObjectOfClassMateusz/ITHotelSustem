using System.ComponentModel.DataAnnotations;

namespace HotelSystemIndustry.Models.Trading
{
    
    public class Purchase
    {
        [Key]
        public Guid Id { get; set; }

        public DateTime TransactionDate { get; set; }

        public ShopPoint? ShopPoint { get; set; }

        public ICollection<SaleItemInstance>? Items { get; set; }
    }

}