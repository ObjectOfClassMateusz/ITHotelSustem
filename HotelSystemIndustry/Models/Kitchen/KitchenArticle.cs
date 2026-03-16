using System.ComponentModel.DataAnnotations;

namespace HotelSystemIndustry.Models.Kitchen
{
    
    public class KitchenArticleType : DictionaryPrototype
    {
        /* Wartości takie jak np.:
        * - DISCRETE,
        * - LOOSE_ARTICLE,
        * - LIQUID
        */
    }


    public class KitchenArticle
    {
        [Key]
        public Guid Id { get; set; }

        [Required, MaxLength(30, ErrorMessage = "Nazwa jest zbyt długa")]
        public required string Name { get; set; }


        public required Guid TypeId { get; set; }

        [Required]
        public virtual required KitchenArticleType Type { get; set; }
    }

}