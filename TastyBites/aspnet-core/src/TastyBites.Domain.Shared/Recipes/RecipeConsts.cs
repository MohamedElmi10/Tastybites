namespace TastyBites.Recipes
{
    // Shared limits for a recipe, used by DTO validation (and the Angular form mirrors them).
    public static class RecipeConsts
    {
        public const int NameMaxLength = 128;
        public const int DescriptionMaxLength = 1000;
        public const int ImageUrlMaxLength = 512;

        // Either a full http(s) link, or a photo we host ourselves in HttpApi.Host/wwwroot/images/recipes.
        // why: self-hosted photos are stored as a relative path, so they keep working when the API moves to a real domain
        public const string ImageUrlPattern = @"^(https?://\S+|/images/recipes/[\w.-]+)$";

        public const int IngredientNameMaxLength = 128;
        public const int IngredientUnitMaxLength = 32;
        public const int StepTextMaxLength = 1000;

        // Upper limits per recipe, so one request can't send thousands of rows
        public const int MaxIngredients = 50;
        public const int MaxSteps = 50;
    }
}
