namespace TastyBites.Recipes
{
    // Numbers for the admin dashboard. The two "Without..." counts are the Admin's to-do list.
    public class RecipeStatsDto
    {
        public int TotalRecipes { get; set; }
        public int WithoutPhoto { get; set; }
        public int WithoutIngredientsOrSteps { get; set; }
    }
}
