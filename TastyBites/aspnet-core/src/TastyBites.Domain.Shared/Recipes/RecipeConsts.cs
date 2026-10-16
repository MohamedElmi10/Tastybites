namespace TastyBites.Recipes
{
    // Shared limits for a recipe, used by DTO validation (and the Angular form mirrors them).
    public static class RecipeConsts
    {
        public const int NameMaxLength = 128;
        public const int DescriptionMaxLength = 1000;
        public const int ImageUrlMaxLength = 512;

        public const int IngredientNameMaxLength = 128;
        public const int IngredientUnitMaxLength = 32;
        public const int StepTextMaxLength = 1000;

        // Upper limits per recipe, so one request can't send thousands of rows
        public const int MaxIngredients = 50;
        public const int MaxSteps = 50;
    }
}
