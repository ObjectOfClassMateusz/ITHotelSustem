using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace HotelSystemIndustry.Models.Kitchen
{

    [PrimaryKey(nameof(RecipeId), nameof(ArticleId))]
    public class KitchenRecipeIngredient
    {
        [Required]
        public Guid RecipeId { get; set; }

        [Required]
        public Guid ArticleId { get; set; }


        public virtual KitchenRecipe? Recipe { get; set; }

        public virtual KitchenArticle? Article { get; set; }

        /*
        * Ułamek może być przydatny dla artykułów sypkich (kg) i cieczy (l).
        * Dla artykułów dyskretnych można przechować po prostu liczbę całkowitą.
        */
        public decimal Count { get; set; }
    }


    public class KitchenRecipe
    {
        [Key]
        public Guid Id { get; set; }


        [Required]
        public Guid OutcomeProductId { get; set; }

        public virtual KitchenProduct? OutcomeProduct { get; set; }


        public virtual ICollection<KitchenRecipeIngredient>? Ingredients { get; set; }


        [Required, MaxLength(10000, ErrorMessage = "Opis przepisu jest zbyt długi")]
        public required string Content { get; set; }
    }

}