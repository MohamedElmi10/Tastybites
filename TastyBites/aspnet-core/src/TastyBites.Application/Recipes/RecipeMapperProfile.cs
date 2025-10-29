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
            CreateMap<Recipe, RecipeDto>().ReverseMap();
        }
    }
}
