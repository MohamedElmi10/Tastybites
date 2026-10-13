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

        // Optional. Must be a full http(s) address when given.
        [Url]
        [StringLength(RecipeConsts.ImageUrlMaxLength)]
        public string? ImageUrl { get; set; }
    }
}
