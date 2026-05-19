using System.ComponentModel.DataAnnotations;

namespace HotelSystemIndustry.ViewModels.Kitchen
{
    
    public class CookWithRecipeTakeArticleViewModel
    {
        [Required]
        public Guid RecipeId { get; set; }

        [Required]
        public Guid ArticleInstanceId { get; set; }

        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal Count { get; set; }

    }

}