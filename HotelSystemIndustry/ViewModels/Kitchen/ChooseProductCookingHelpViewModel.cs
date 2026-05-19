using HotelSystemIndustry.Models.Kitchen;

namespace HotelSystemIndustry.ViewModels.Kitchen
{
    public class ChooseProductCookingHelpViewModel
    {
        public Guid ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public IList<KitchenRecipe> ProductRecipes { get; set; } = new List<KitchenRecipe>();
    }
}