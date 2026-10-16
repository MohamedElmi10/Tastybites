using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace TastyBites.Recipes
{
    public interface IRecipeAdminAppService : ICrudAppService<
    RecipeDto,
    int,
    PagedAndSortedResultRequestDto>
    {
        // Create Read(Single, list) Update, Delete
        Task<List<RecipeDto>> GetRecentAsync(int count = 3);
        Task<List<RecipeDto>> GetAllRecipesAsync();
        Task<RecipeStatsDto> GetStatsAsync();
    }
}