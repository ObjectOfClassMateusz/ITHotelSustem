using System.ComponentModel.DataAnnotations;

namespace HotelSystemIndustry.ViewModels.Kitchen
{
    public class KitchenIngredientEditViewModel
    {
        [Required]
        public Guid ArticleId { get; set; }

        /*
        * Ułamek może być przydatny dla artykułów sypkich (kg) i cieczy (l).
        * Dla artykułów dyskretnych można przechować po prostu liczbę całkowitą.
        */
        [Range(typeof(decimal), "0", "79228162514264337593543950335"), Display(Name="Count")]
        public decimal Count { get; set; } = 1;
    }
    
    public class KitchenRecipeEditViewModel
    {
        // Used only when editing an existing recipe
        public Guid TargetRecipeId { get; set; } = Guid.Empty;
        
        [Required]
        public Guid OutcomeProductId { get; set; }

        [Required, MaxLength(10000, ErrorMessage = "Opis przepisu jest zbyt długi")]
        public string Content { get; set; } = string.Empty;


        public IList<KitchenIngredientEditViewModel> Ingredients { get; set; } = new List<KitchenIngredientEditViewModel>();


        public KitchenIngredientEditViewModel NewIngredient {get; set; } = new();
    }

}