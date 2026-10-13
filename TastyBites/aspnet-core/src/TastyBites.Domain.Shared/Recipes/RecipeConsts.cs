namespace TastyBites.Recipes
{
    // Shared limits for a recipe, used by DTO validation (and the Angular form mirrors them).
    public static class RecipeConsts
    {
        public const int NameMaxLength = 128;
        public const int DescriptionMaxLength = 1000;
        public const int ImageUrlMaxLength = 512;
    }
}
