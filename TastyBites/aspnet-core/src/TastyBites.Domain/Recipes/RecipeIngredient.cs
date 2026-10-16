using Volo.Abp.Domain.Entities;

namespace TastyBites.Recipes
{
    // One line in a recipe's ingredient list, e.g. "200 g chicken thighs".
    // Belongs to exactly one Recipe (RecipeId) and is deleted together with it.
    public class RecipeIngredient : Entity<int>
    {
        public int RecipeId { get; set; }

        public string Name { get; set; } = string.Empty;   // "chicken thighs"
        public decimal? Amount { get; set; }               // 200 (optional: "salt to taste")
        public string? Unit { get; set; }                  // "g", "dl", "pcs" (optional)

        public int SortOrder { get; set; }                 // position in the list, 0 = first
    }
}
