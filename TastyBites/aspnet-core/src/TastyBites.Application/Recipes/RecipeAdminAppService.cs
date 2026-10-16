using System;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using TastyBites.Permissions;

namespace TastyBites.Recipes
{
    // Admin only: every method needs at least the Recipes permission (must be logged in).
    [Authorize(TastyBitesPermissions.Recipes.Default)]
    public class RecipeAdminAppService : CrudAppService<Recipe, RecipeDto, int, PagedAndSortedResultRequestDto>, IRecipeAdminAppService
    {
        private readonly IRepository<Recipe, int> _recipesRepository;

        public RecipeAdminAppService(
            IRepository<Recipe, int> recipesRepository
        )
        : base(recipesRepository)
        {
            this._recipesRepository = recipesRepository;
        }

        //overiding a public method/function from the base class
        public override  async Task<RecipeDto> GetAsync(int id){
            var recipe = await _recipesRepository.GetAsync(id);
            var recipeDto = ObjectMapper.Map<Recipe, RecipeDto>(recipe);
            return recipeDto;
        }

        //Create a new recipe ovverrid
        [Authorize(TastyBitesPermissions.Recipes.Create)]
        public override async Task<RecipeDto> CreateAsync(RecipeDto input){
            var recipe = ObjectMapper.Map<RecipeDto, Recipe>(input);
            recipe.Name = recipe.Name.Trim();
            recipe.Description = recipe.Description.Trim();
            recipe.ImageUrl = recipe.ImageUrl?.Trim();
            ReplaceIngredientsAndSteps(recipe, input);
            await _recipesRepository.InsertAsync(recipe, autoSave: true);
            var recipeDto = ObjectMapper.Map<Recipe, RecipeDto>(recipe);
            return recipeDto;
        }

        //update recipe
        [Authorize(TastyBitesPermissions.Recipes.Edit)]
        public override async Task<RecipeDto> UpdateAsync(int id, RecipeDto input){
            var recipe =  await _recipesRepository.GetAsync(id);
            //only the availabe values from the input DTO will be applied To the recipe entity
            //IMPORTANT: Any values not present in the DTO will remain unchanged in the recipe.

            ObjectMapper.Map<RecipeDto,Recipe>(input,recipe);
            ReplaceIngredientsAndSteps(recipe, input);
            await _recipesRepository.UpdateAsync(recipe,autoSave: true);
            var recipeDto = ObjectMapper.Map<Recipe, RecipeDto>(recipe);
            return recipeDto;
        }

        // why: throw away the old lists and save the new ones instead of matching item by item -
        // max 50 rows each, so it's cheap and much simpler (docs/adr/0001).
        // Removing a child from the loaded list makes EF delete its row on save.
        private static void ReplaceIngredientsAndSteps(Recipe recipe, RecipeDto input)
        {
            recipe.Ingredients.Clear();
            var ingredients = input.Ingredients ?? new List<RecipeIngredientDto>(); // why: JSON "ingredients": null would otherwise crash
            for (var i = 0; i < ingredients.Count; i++)
            {
                recipe.Ingredients.Add(new RecipeIngredient
                {
                    Name = ingredients[i].Name.Trim(),
                    Amount = ingredients[i].Amount,
                    Unit = string.IsNullOrWhiteSpace(ingredients[i].Unit) ? null : ingredients[i].Unit!.Trim(),
                    SortOrder = i // list position = order on the page
                });
            }

            recipe.Steps.Clear();
            var steps = input.Steps ?? new List<RecipeStepDto>();
            for (var i = 0; i < steps.Count; i++)
            {
                recipe.Steps.Add(new RecipeStep { Text = steps[i].Text.Trim(), SortOrder = i });
            }
        }
        //override the delete method
        [Authorize(TastyBitesPermissions.Recipes.Delete)]
        public override async Task DeleteAsync (int id){
            await _recipesRepository.DeleteAsync(id);
        }
        //overriding-the-read-operation-paged-list
        public override async Task<PagedResultDto<RecipeDto>> GetListAsync (PagedAndSortedResultRequestDto input){
            var totalCount = await _recipesRepository.GetCountAsync();

            var recipes = await _recipesRepository.GetPagedListAsync(
                input.SkipCount,
                input.MaxResultCount,
                input.Sorting  ?? nameof(Recipe.Name)
            );
            var recipeDtos = ObjectMapper.Map< List<Recipe>, List<RecipeDto>>(recipes);
            var PagedResultDto= new PagedResultDto<RecipeDto>(totalCount,recipeDtos);
            return PagedResultDto;
        }

        // Custom method: newest recipes first (highest Id = most recently added)
        public async Task<List<RecipeDto>> GetRecentAsync(int count = 3)
        {
            count = Math.Clamp(count, 1, 50);
            var queryable = await _recipesRepository.GetQueryableAsync();
            var recipes = await AsyncExecuter.ToListAsync(
                queryable.OrderByDescending(r => r.Id).Take(count)
            );
            return ObjectMapper.Map<List<Recipe>, List<RecipeDto>>(recipes);
        }

        // All recipes in one call (no paging), sorted by name. Used by the admin recipes list.
        public async Task<List<RecipeDto>> GetAllRecipesAsync()
        {
            var queryable = await _recipesRepository.GetQueryableAsync();
            var recipes = await AsyncExecuter.ToListAsync(queryable.OrderBy(r => r.Name));
            return ObjectMapper.Map<List<Recipe>, List<RecipeDto>>(recipes);
        }
    
    }
}