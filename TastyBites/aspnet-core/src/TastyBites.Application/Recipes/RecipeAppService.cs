using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Domain.Repositories;

namespace TastyBites.Recipes
{
    // Public, read-only. ABP exposes it as /api/app/recipe.
    // Writing recipes stays in RecipeAdminAppService behind permissions.
    [AllowAnonymous]
    public class RecipeAppService : TastyBitesAppService, IRecipeAppService
    {
        private readonly IRepository<Recipe, int> _recipesRepository;

        public RecipeAppService(IRepository<Recipe, int> recipesRepository)
        {
            _recipesRepository = recipesRepository;
        }

        public async Task<List<RecipeDto>> GetListAsync()
        {
            var queryable = await _recipesRepository.GetQueryableAsync();
            var recipes = await AsyncExecuter.ToListAsync(queryable.OrderBy(r => r.Name));
            return ObjectMapper.Map<List<Recipe>, List<RecipeDto>>(recipes);
        }

        public async Task<RecipeDto> GetAsync(int id)
        {
            var recipe = await _recipesRepository.GetAsync(id); // 404 if it doesn't exist
            return ObjectMapper.Map<Recipe, RecipeDto>(recipe);
        }

        public async Task<List<RecipeDto>> GetRecentAsync(int count = 3)
        {
            count = Math.Clamp(count, 1, 12);
            var queryable = await _recipesRepository.GetQueryableAsync();
            var recipes = await AsyncExecuter.ToListAsync(
                queryable.OrderByDescending(r => r.Id).Take(count)
            );
            return ObjectMapper.Map<List<Recipe>, List<RecipeDto>>(recipes);
        }
    }
}
