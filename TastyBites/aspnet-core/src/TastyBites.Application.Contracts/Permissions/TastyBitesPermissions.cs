namespace TastyBites.Permissions;

public static class TastyBitesPermissions
{
    public const string GroupName = "TastyBites";

    // Managing recipes in the admin app. Reading recipes on the public site needs no permission.
    public static class Recipes
    {
        public const string Default = GroupName + ".Recipes";   // open the admin recipes pages
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }
}
