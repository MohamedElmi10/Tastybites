using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace TastyBites.Recipes
{
    public class Recipe : Entity<int>

    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        // Link to a photo of the dish. Optional: recipes without one show a placeholder.
        public string? ImageUrl { get; set; }

        // One-to-many: a recipe has many ingredients and many steps
        public List<RecipeIngredient> Ingredients { get; set; } = new();
        public List<RecipeStep> Steps { get; set; } = new();
    }
}