using System.Text.Json;

namespace HotelSystemIndustry.Services
{
    
    public class ExternalRecipeService : IExternalRecipeService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiKey = "1"; // Klucz testowy


        public ExternalRecipeService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;

            _apiKey = configuration.GetValue<string>("themealdbKey")!;
        }


        public async Task<IList<ExternalMealCategory>> GetCategories()
        {
            var client = _httpClientFactory.CreateClient("themealdb");
            var response = await client.GetAsync($"{_apiKey}/categories.php");

            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var result = await response.Content.ReadAsStringAsync();
                var categories = JsonSerializer.Deserialize<ExternalMealCategories>(result, options);
                if (categories != null && categories.Categories != null)
                    return categories.Categories;
            }

            return new List<ExternalMealCategory>();
        }

        public async Task<IList<ExternalMealArea>> GetAreas()
        {
            var client = _httpClientFactory.CreateClient("themealdb");
            var response = await client.GetAsync($"{_apiKey}/list.php?a=list");

            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var result = await response.Content.ReadAsStringAsync();
                var areas = JsonSerializer.Deserialize<ExternalMealAreas>(result, options);
                if (areas != null && areas.Meals != null)
                    return areas.Meals;
            }

            return new List<ExternalMealArea>();
        }



        public async Task<IList<ExternalFilteredMeal>> GetMealsOfCategory(string category)
        {
            var client = _httpClientFactory.CreateClient("themealdb");
            var response = await client.GetAsync($"{_apiKey}/filter.php?c={category}");

            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var result = await response.Content.ReadAsStringAsync();
                var meals = JsonSerializer.Deserialize<ExternalFilteredMeals>(result, options);
                if (meals != null && meals.Meals != null)
                    return meals.Meals;
            }

            return new List<ExternalFilteredMeal>();
        }


        public async Task<IList<ExternalFilteredMeal>> GetMealsFromArea(string area)
        {
            var client = _httpClientFactory.CreateClient("themealdb");
            var response = await client.GetAsync($"{_apiKey}/filter.php?a={area}");

            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var result = await response.Content.ReadAsStringAsync();
                var meals = JsonSerializer.Deserialize<ExternalFilteredMeals>(result, options);
                if (meals != null && meals.Meals != null)
                    return meals.Meals;
            }

            return new List<ExternalFilteredMeal>();
        }


        public async Task<IList<ExternalMeal>> SearchMealsByName(string name)
        {
            var client = _httpClientFactory.CreateClient("themealdb");
            var response = await client.GetAsync($"{_apiKey}/search.php?s={name}");

            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var result = await response.Content.ReadAsStringAsync();
                var meals = JsonSerializer.Deserialize<ExternalMeals>(result, options);
                if (meals != null && meals.Meals != null)
                    return meals.Meals;
            }

            return new List<ExternalMeal>();
        }

        public async Task<ExternalMeal?> GetMealDetails(string id)
        {
            var client = _httpClientFactory.CreateClient("themealdb");
            var response = await client.GetAsync($"{_apiKey}/lookup.php?i={id}");

            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var result = await response.Content.ReadAsStringAsync();
                var meals = JsonSerializer.Deserialize<ExternalMeals>(result, options);
                if (meals != null && meals.Meals != null && meals.Meals.Count > 0)
                    return meals.Meals[0];
            }

            return null;
        }

        public async Task<ExternalMeal?> GetRandomMeal()
        {
            var client = _httpClientFactory.CreateClient("themealdb");
            var response = await client.GetAsync($"{_apiKey}/random.php");

            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var result = await response.Content.ReadAsStringAsync();
                var meals = JsonSerializer.Deserialize<ExternalMeals>(result, options);
                if (meals != null && meals.Meals != null && meals.Meals.Count > 0)
                    return meals.Meals[0];
            }

            return null;
        }


        private class ExternalMealCategories
        {
            public IList<ExternalMealCategory>? Categories { get; set; }
        }

        private class ExternalMealAreas
        {
            public IList<ExternalMealArea>? Meals { get; set; }
        }

        private class ExternalFilteredMeals
        {
            public IList<ExternalFilteredMeal>? Meals { get; set; }
        }

        private class ExternalMeals
        {
            public IList<ExternalMeal>? Meals { get; set; }
        }
    }

}