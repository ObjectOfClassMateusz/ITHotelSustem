namespace HotelSystemIndustry.Services
{
    public class ExternalMealCategory
    {
        public string IdCategory { get; set; } = string.Empty;

        public string StrCategory { get; set; } = string.Empty;

        public string StrCategoryThumb { get; set; } = string.Empty;

        public string StrCategoryDescription { get; set; } = string.Empty;
    }


    public class ExternalMealArea
    {
        public string StrArea { get; set; } = string.Empty;

        public string StrCountry { get; set; } = string.Empty;
    }


    public class ExternalFilteredMeal
    {
        public string StrMeal { get; set; } = string.Empty;

        public string StrMealThumb { get; set; } = string.Empty;
        
        public string IdMeal { get; set; } = string.Empty;
        
        public string? StrArea { get; set; }

        public string StrCountry { get; set; } = string.Empty;
    }


    public class ExternalMeal
    {
        public string IdMeal { get; set; } = string.Empty;

        public string StrMeal { get; set; } = string.Empty;

        public string StrCategory { get; set; } = string.Empty;

        public string? StrArea { get; set; }

        public string StrCountry { get; set; } = string.Empty;

        public string StrInstructions { get; set; } = string.Empty;

        public string StrMealThumb { get; set; } = string.Empty;
        
        public string? StrTags { get; set; }

        public string StrYoutube { get; set; } = string.Empty;


        public string? StrIngredient1 { get; set; }
        public string? StrIngredient2 { get; set; }
        public string? StrIngredient3 { get; set; }
        public string? StrIngredient4 { get; set; }
        public string? StrIngredient5 { get; set; }
        public string? StrIngredient6 { get; set; }
        public string? StrIngredient7 { get; set; }
        public string? StrIngredient8 { get; set; }
        public string? StrIngredient9 { get; set; }
        public string? StrIngredient10 { get; set; }
        public string? StrIngredient11 { get; set; }
        public string? StrIngredient12 { get; set; }
        public string? StrIngredient13 { get; set; }
        public string? StrIngredient14 { get; set; }
        public string? StrIngredient15 { get; set; }
        public string? StrIngredient16 { get; set; }
        public string? StrIngredient17 { get; set; }
        public string? StrIngredient18 { get; set; }
        public string? StrIngredient19 { get; set; }
        public string? StrIngredient20 { get; set; }

        public string? StrMeasure1 { get; set; }
        public string? StrMeasure2 { get; set; }
        public string? StrMeasure3 { get; set; }
        public string? StrMeasure4 { get; set; }
        public string? StrMeasure5 { get; set; }
        public string? StrMeasure6 { get; set; }
        public string? StrMeasure7 { get; set; }
        public string? StrMeasure8 { get; set; }
        public string? StrMeasure9 { get; set; }
        public string? StrMeasure10 { get; set; }
        public string? StrMeasure11 { get; set; }
        public string? StrMeasure12 { get; set; }
        public string? StrMeasure13 { get; set; }
        public string? StrMeasure14 { get; set; }
        public string? StrMeasure15 { get; set; }
        public string? StrMeasure16 { get; set; }
        public string? StrMeasure17 { get; set; }
        public string? StrMeasure18 { get; set; }
        public string? StrMeasure19 { get; set; }
        public string? StrMeasure20 { get; set; }
    }


    public interface IExternalRecipeService
    {
        Task<IList<ExternalMealCategory>> GetCategories();

        Task<IList<ExternalMealArea>> GetAreas();

        Task<IList<ExternalFilteredMeal>> GetMealsOfCategory(string category);

        Task<IList<ExternalFilteredMeal>> GetMealsFromArea(string area);

        Task<IList<ExternalMeal>> SearchMealsByName(string name);

        Task<ExternalMeal?> GetMealDetails(string id);

        Task<ExternalMeal?> GetRandomMeal();
    }
}