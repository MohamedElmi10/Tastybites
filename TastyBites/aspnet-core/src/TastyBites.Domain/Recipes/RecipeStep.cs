using Volo.Abp.Domain.Entities;

namespace TastyBites.Recipes
{
    // One instruction in a recipe, e.g. "Brown the chicken for 5 minutes."
    // Shown to visitors as a numbered list, ordered by SortOrder.
    public class RecipeStep : Entity<int>
    {
        public int RecipeId { get; set; }

        public string Text { get; set; } = string.Empty;

        public int SortOrder { get; set; }                 // 0 = step 1
    }
}
