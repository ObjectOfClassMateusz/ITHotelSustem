using System.ComponentModel.DataAnnotations;

namespace HotelSystemIndustry.Models.Kitchen
{
    
    public enum KitchenArticleType
    {
        DISCRETE,
        LOOSE_ARTICLE,
        LIQUID
    }


    public class KitchenArticle
    {
        [Key]
        public Guid Id { get; set; }

        public required string Name { get; set; }

        public KitchenArticleType Type { get; set; }
    }

}