using System.ComponentModel.DataAnnotations;

namespace HotelSystemIndustry.Models.Trading
{

    public class SaleItemType : DictionaryPrototype
    {
        /* np.:
        * - TO_BUY,
        * - FOR_DAY_LEASE,
        * - FOR_MONTHLY_LEASE
        */
    }
    
    public class SaleItem
    {
        [Key]
        public Guid Id { get; set; }

        [Required, MaxLength(30, ErrorMessage = "Nazwa jest zbyt długa")]
        public required string Name { get; set; }


        [Required]
        public Guid TypeId { get; set; }

        public virtual SaleItemType? Type { get; set; }


        public bool ContainsAlcohol { get; set; }
    }

}