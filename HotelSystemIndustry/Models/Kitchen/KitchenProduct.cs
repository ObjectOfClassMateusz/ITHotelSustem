using System.ComponentModel.DataAnnotations;

namespace HotelSystemIndustry.Models.Kitchen
{

    public class KitchenProduct
    {
        [Key]
        public Guid Id { get; set; }

        public required string Name { get; set; }

        public bool ContainsAlcohol { get; set; }

        public decimal Price { get; set; }
    }

}