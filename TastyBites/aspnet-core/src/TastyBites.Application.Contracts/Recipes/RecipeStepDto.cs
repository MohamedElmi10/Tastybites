using System.ComponentModel.DataAnnotations;

namespace TastyBites.Recipes
{
    // One step inside RecipeDto.Steps. Step number = position in the list.
    // why: no Id or SortOrder, same reason as RecipeIngredientDto (docs/adr/0001)
    public class RecipeStepDto
    {
        [Required]
        [StringLength(RecipeConsts.StepTextMaxLength)]
        public string Text { get; set; } = string.Empty;
    }
}
