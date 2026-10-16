using System.ComponentModel.DataAnnotations;

namespace TastyBites.Recipes
{
    // One ingredient inside RecipeDto.Ingredients.
    // why: no Id or SortOrder - the list position is the order, and update replaces the whole list (docs/adr/0001)
    public class RecipeIngredientDto
    {
        [Required]
        [StringLength(RecipeConsts.IngredientNameMaxLength)]
        public string Name { get; set; } = string.Empty;

        // why: same range as the database column decimal(10,2), so a too-big number fails with 400 instead of a DB error
        [Range(0, 99999999.99)]
        public decimal? Amount { get; set; }

        [StringLength(RecipeConsts.IngredientUnitMaxLength)]
        public string? Unit { get; set; }
    }
}
