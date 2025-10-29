using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace TastyBites.Recipes
{
    public class RecipeAdminAppService : CrudAppService<Recipe, RecipeDto, int, PagedAndSortedResultRequestDto>, IRecipeAppService
    {
        public RecipeAdminAppService(
            IRepository<Recipe, int> repository
        )
        : base(repository)
        {
            
        }
    }
}