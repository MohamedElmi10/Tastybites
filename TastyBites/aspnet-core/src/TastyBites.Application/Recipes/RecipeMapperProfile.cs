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
            CreateMap<Recipe, RecipeDto>();

            // DTO -> entity: never copy Id from the request.
            // The id comes from the URL; if the body has no id (0), copying it would try to change the primary key.
            CreateMap<RecipeDto, Recipe>()
                .ForMember(recipe => recipe.Id, options => options.Ignore());
        }
    }
}
