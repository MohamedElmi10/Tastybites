using Volo.Abp.Application.Dtos;

namespace TastyBites.Recipes
{
    public class RecipeDto : EntityDto<int>

    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    
    }
}