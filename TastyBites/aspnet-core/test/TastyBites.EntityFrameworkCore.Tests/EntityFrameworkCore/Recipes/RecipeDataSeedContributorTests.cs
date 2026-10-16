using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using TastyBites.Recipes;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace TastyBites.EntityFrameworkCore.Recipes;

// Runs against the in-memory SQLite test database. The test base already ran all data seeders
// (including RecipeDataSeedContributor) on an empty database before each test.
public class RecipeDataSeedContributorTests : TastyBitesEntityFrameworkCoreTestBase
{
    private const int SeededRecipeCount = 12; // entries in Recipes/Seed/recipe-seed-data.json

    private readonly IRepository<Recipe, int> _recipesRepository;
    private readonly IDataSeeder _dataSeeder;

    public RecipeDataSeedContributorTests()
    {
        _recipesRepository = GetRequiredService<IRepository<Recipe, int>>();
        _dataSeeder = GetRequiredService<IDataSeeder>();
    }

    [Fact]
    public async Task Should_Seed_Recipes_With_Ingredients_And_Steps_In_Order()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            var queryable = await _recipesRepository.WithDetailsAsync(r => r.Ingredients, r => r.Steps);
            var recipes = await queryable.ToListAsync();

            recipes.Count.ShouldBe(SeededRecipeCount);
            recipes.ShouldAllBe(r => r.Ingredients.Count > 0 && r.Steps.Count > 0);

            var carbonara = recipes.Single(r => r.Name == "Spaghetti Carbonara");
            carbonara.Ingredients.OrderBy(i => i.SortOrder).First().Name.ShouldBe("spaghetti");
            carbonara.Steps.OrderBy(s => s.SortOrder).Select(s => s.SortOrder).ShouldBe(Enumerable.Range(0, carbonara.Steps.Count));
        });
    }

    [Fact]
    public async Task Should_Not_Duplicate_Recipes_When_Seeding_Again()
    {
        // Same as running ./scripts/dev.sh migrate a second time
        await _dataSeeder.SeedAsync();

        (await _recipesRepository.GetCountAsync()).ShouldBe(SeededRecipeCount);
    }
}
