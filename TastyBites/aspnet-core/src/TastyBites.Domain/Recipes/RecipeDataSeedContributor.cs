using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace TastyBites.Recipes
{
    // Fills an empty database with the starter recipes from Recipes/Seed/recipe-seed-data.json.
    // ABP runs every IDataSeedContributor when DbMigrator runs (./scripts/dev.sh migrate).
    public class RecipeDataSeedContributor : IDataSeedContributor, ITransientDependency
    {
        public const string ResourceName = "TastyBites.Recipes.recipe-seed-data.json"; // see LogicalName in the .csproj

        private readonly IRepository<Recipe, int> _recipesRepository;

        public RecipeDataSeedContributor(IRepository<Recipe, int> recipesRepository)
        {
            _recipesRepository = recipesRepository;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            // why: only seed an empty table - running migrate again must never duplicate
            // or overwrite recipes the Admin has created or edited
            if (await _recipesRepository.GetCountAsync() > 0)
            {
                return;
            }

            foreach (var seed in LoadSeedData())
            {
                var recipe = new Recipe
                {
                    Name = seed.Name,
                    Description = seed.Description,
                    ImageUrl = seed.ImageUrl,
                };
                // List position = order on the page, same rule as RecipeAdminAppService (docs/adr/0001)
                for (var i = 0; i < seed.Ingredients.Count; i++)
                {
                    var ingredient = seed.Ingredients[i];
                    recipe.Ingredients.Add(new RecipeIngredient
                    {
                        Name = ingredient.Name,
                        Amount = ingredient.Amount,
                        Unit = ingredient.Unit,
                        SortOrder = i,
                    });
                }
                for (var i = 0; i < seed.Steps.Count; i++)
                {
                    recipe.Steps.Add(new RecipeStep { Text = seed.Steps[i], SortOrder = i });
                }

                await _recipesRepository.InsertAsync(recipe);
            }
        }

        private static List<RecipeSeed> LoadSeedData()
        {
            using var stream = typeof(RecipeDataSeedContributor).Assembly.GetManifestResourceStream(ResourceName)
                ?? throw new InvalidOperationException($"Embedded resource '{ResourceName}' not found.");

            return JsonSerializer.Deserialize<List<RecipeSeed>>(
                stream,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            ) ?? new List<RecipeSeed>();
        }

        // Shape of one entry in recipe-seed-data.json
        private record RecipeSeed(
            string Name,
            string Description,
            string? ImageUrl,
            List<IngredientSeed> Ingredients,
            List<string> Steps
        );

        private record IngredientSeed(string Name, decimal? Amount, string? Unit);
    }
}
