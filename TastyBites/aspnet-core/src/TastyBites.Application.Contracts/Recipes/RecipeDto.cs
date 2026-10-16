using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace TastyBites.Recipes
{
    public class RecipeDto : EntityDto<int>
    {
        // Server-side validation: ABP rejects the request with a 400 before our code runs
        [Required]
        [StringLength(RecipeConsts.NameMaxLength)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(RecipeConsts.DescriptionMaxLength)]
        public string Description { get; set; } = string.Empty;

        // Optional. A full http(s) address, or /images/recipes/<file> for a self-hosted photo.
        [RegularExpression(RecipeConsts.ImageUrlPattern)]
        [StringLength(RecipeConsts.ImageUrlMaxLength)]
        public string? ImageUrl { get; set; }

        // Only filled when fetching ONE recipe (GetAsync); list endpoints return them empty.
        // why: MaxLength on a list limits the item count, so one request can't insert thousands of rows
        [MaxLength(RecipeConsts.MaxIngredients)]
        public List<RecipeIngredientDto> Ingredients { get; set; } = new();

        [MaxLength(RecipeConsts.MaxSteps)]
        public List<RecipeStepDto> Steps { get; set; } = new();
    }
}
