using System.ComponentModel.DataAnnotations;

namespace HotelSystemIndustry.Models.Kitchen
{

    public class KitchenRecipe
    {
        [Key]
        public Guid Id { get; set; }

        public required KitchenProduct OutcomeProduct { get; set; }

        public struct Ingredient
        {
            public KitchenArticle article;

            /*
            * Ułamek może być przydatny dla artykułów sypkich (kg) i cieczy (l).
            * Dla artykułów dyskretnych można przechować po prostu liczbę całkowitą.
            */
            public decimal Count;
        }

        public ICollection<Ingredient>? Ingredients { get; set; }

        public required string Content { get; set; }
    }

}