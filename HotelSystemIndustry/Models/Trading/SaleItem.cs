using System.ComponentModel.DataAnnotations;

namespace HotelSystemIndustry.Models.Trading
{

    public enum SaleItemType
    {
        TO_BUY,
        FOR_DAY_LEASE,
        FOR_MONTHLY_LEASE
    }
    
    public class SaleItem
    {
        [Key]
        public Guid Id { get; set; }

        public required string Name { get; set; }

        public SaleItemType Type { get; set; }

        public bool ContainsAlcohol { get; set; }
    }

}