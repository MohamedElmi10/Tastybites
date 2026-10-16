using System.Threading.Tasks;
using Shouldly;
using TastyBites.Recipes;
using Xunit;

namespace TastyBites.Recipes;

// Uses the starter recipes from RecipeDataSeedContributor (seeded into the in-memory test database).
public class RecipeAdminAppServiceTests : TastyBitesApplicationTestBase
{
    private readonly IRecipeAdminAppService _recipeAdminAppService;

    public RecipeAdminAppServiceTests()
    {
        _recipeAdminAppService = GetRequiredService<IRecipeAdminAppService>();
    }

    [Fact]
    public async Task GetStats_Should_Count_Seeded_Recipes()
    {
        var stats = await _recipeAdminAppService.GetStatsAsync();

        stats.TotalRecipes.ShouldBe(15);
        stats.WithoutPhoto.ShouldBe(1);               // İskender Kebab has no photo in the seed data
        stats.WithoutIngredientsOrSteps.ShouldBe(0);  // every seeded recipe has both lists
    }

    [Fact]
    public async Task GetStats_Should_Count_Recipe_Without_Lists()
    {
        await _recipeAdminAppService.CreateAsync(new RecipeDto { Name = "Toast", Description = "Just toast." });

        var stats = await _recipeAdminAppService.GetStatsAsync();

        stats.TotalRecipes.ShouldBe(16);
        stats.WithoutPhoto.ShouldBe(2);
        stats.WithoutIngredientsOrSteps.ShouldBe(1);
    }
}
