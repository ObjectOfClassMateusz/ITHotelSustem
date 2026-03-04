using System.ComponentModel.DataAnnotations;

namespace HotelSystemIndustry.Models.Housekeeping
{
    public enum SupplyCategory
    {
        CleaningProducts,
        GuestAmenities,
        LinensAndTowels
    }

    public enum SupplyUnit
    {
        Pieces,
        Liters,
        Kilograms
    }

    public class HousekeepingSupply
    {
        [Key]
        public Guid Id { get; set; }

        public required string Name { get; set; }

        public SupplyCategory Category { get; set; }

        public SupplyUnit Unit { get; set; }
        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal QuantityInStock { get; set; }
        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal MinimumRequiredQuantity { get; set; }
    }
}