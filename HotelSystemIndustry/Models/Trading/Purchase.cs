using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace HotelSystemIndustry.Models.Trading
{

    public class PurchaseItem
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid PurchaseId { get; set; }

        public virtual Purchase? Purchase { get; set; }

        [Required]
        public Guid SaleItemId { get; set; }

        public virtual SaleItem? SaleItem { get; set; }


        public uint Count { get; set; }

        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString="{0:C2}")]
        [Range(typeof(decimal), "0", "10000")]
        public decimal UnitPrice { get; set; }

        public string Variant { get; set; } = string.Empty;


        public bool HasBeenReturned { get; set; } = false;
    }

    
    public class Purchase
    {
        [Key]
        public Guid Id { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime TransactionDate { get; set; }

        public Guid? ShopPointId { get; set; }
        
        public virtual ShopPoint? ShopPoint { get; set; }

        public virtual ICollection<PurchaseItem>? Items { get; set; }
    }

}
