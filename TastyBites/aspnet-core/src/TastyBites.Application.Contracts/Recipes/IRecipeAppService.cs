using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace TastyBites.Recipes
{
    // Read-only recipes for visitors on the public site. No login needed.
    public interface IRecipeAppService : IApplicationService
    {
        Task<List<RecipeDto>> GetListAsync();
        Task<RecipeDto> GetAsync(int id);
        Task<List<RecipeDto>> GetRecentAsync(int count = 3);
    }
}
