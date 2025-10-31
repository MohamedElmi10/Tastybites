using System;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace TastyBites.Recipes
{
    public class RecipeAdminAppService : CrudAppService<Recipe, RecipeDto, int, PagedAndSortedResultRequestDto>, IRecipeAppService
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
            // custom logic here
            recipe.Name = recipe.Name.Substring(1);
            var recipeDto = ObjectMapper.Map<Recipe, RecipeDto>(recipe);
            return recipeDto;
        }

        //Create a new recipe ovverrid
        public override async Task<RecipeDto> CreateAsync(RecipeDto input){
            var recipe = ObjectMapper.Map<RecipeDto, Recipe>(input);
            recipe.Name = recipe.Name.Trim();
            recipe.Name = recipe.Description.Trim();
            await _recipesRepository.InsertAsync(recipe, autoSave: true);
            var recipeDto = ObjectMapper.Map<Recipe, RecipeDto>(recipe);
            return recipeDto;
        }

        //update recipe
        public override async Task<RecipeDto> UpdateAsync(int id, RecipeDto input){
            var recipe =  await _recipesRepository.GetAsync(id);
            //only the availabe values from the input DTO will be applied To the recipe entity
            //IMPORTANT: Any values not present in the DTO will remain unchanged in the recipe.

            ObjectMapper.Map<RecipeDto,Recipe>(input,recipe);
            await _recipesRepository.UpdateAsync(recipe,autoSave: true);
            var recipeDto = ObjectMapper.Map<Recipe, RecipeDto>(recipe);
            return recipeDto;
        }
        //override the delete method
        public override async Task DeleteAsync (int id){
            var recipe = await _recipesRepository.GetAsync(id);

            // Custom logic
            if(recipe.Name.Contains("Butter Chicken", StringComparison.OrdinalIgnoreCase)){
                throw new UserFriendlyException("You cannot delete chicken recipes");
            }
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
    
    }
}