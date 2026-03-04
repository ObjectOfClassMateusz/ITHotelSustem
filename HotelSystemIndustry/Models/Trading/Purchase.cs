using System.ComponentModel.DataAnnotations;

namespace HotelSystemIndustry.Models.Trading
{
    
    public class Purchase
    {
        [Key]
        public Guid Id { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime TransactionDate { get; set; }
        [Required]
        public ShopPoint? ShopPoint { get; set; }

        public ICollection<SaleItemInstance>? Items { get; set; }
    }

}