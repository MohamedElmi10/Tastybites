using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace TastyBites.Recipes
{
    public class RecipeMapperProfile : Profile
    {
        public RecipeMapperProfile()
        {
            // why: sort here so every endpoint returns ingredients and steps in the Admin's order, however they were loaded
            CreateMap<Recipe, RecipeDto>()
                .ForMember(dto => dto.Ingredients, options => options.MapFrom(recipe => recipe.Ingredients.OrderBy(i => i.SortOrder)))
                .ForMember(dto => dto.Steps, options => options.MapFrom(recipe => recipe.Steps.OrderBy(st => st.SortOrder)));
            CreateMap<RecipeIngredient, RecipeIngredientDto>();
            CreateMap<RecipeStep, RecipeStepDto>();

            // DTO -> entity: never copy Id from the request.
            // The id comes from the URL; if the body has no id (0), copying it would try to change the primary key.
            // why: lists are skipped too - RecipeAdminAppService rebuilds them itself (docs/adr/0001)
            CreateMap<RecipeDto, Recipe>()
                .ForMember(recipe => recipe.Id, options => options.Ignore())
                .ForMember(recipe => recipe.Ingredients, options => options.Ignore())
                .ForMember(recipe => recipe.Steps, options => options.Ignore());
        }
    }
}
