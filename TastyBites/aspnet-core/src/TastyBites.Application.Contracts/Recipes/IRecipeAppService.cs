using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace TastyBites.Recipes
{
    public interface IRecipeAppService : ICrudAppService<
    RecipeDto,
    int,
    PagedAndSortedResultRequestDto>
    {
        // Create Read(Single, list) Update, Delete
    }
}