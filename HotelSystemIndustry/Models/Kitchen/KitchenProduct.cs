using System.ComponentModel.DataAnnotations;

namespace HotelSystemIndustry.Models.Kitchen
{

    public class KitchenProduct
    {
        [Key]
        public Guid Id { get; set; }

        public required string Name { get; set; }

        public bool ContainsAlcohol { get; set; }
        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal Price { get; set; }
    }

}