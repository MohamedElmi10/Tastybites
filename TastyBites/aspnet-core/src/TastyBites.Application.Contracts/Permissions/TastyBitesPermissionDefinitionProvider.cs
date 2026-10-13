using TastyBites.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace TastyBites.Permissions;

public class TastyBitesPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var group = context.AddGroup(TastyBitesPermissions.GroupName, L("Permission:TastyBites"));

        // Parent permission + children. A child can only be granted together with its parent.
        var recipes = group.AddPermission(TastyBitesPermissions.Recipes.Default, L("Permission:Recipes"));
        recipes.AddChild(TastyBitesPermissions.Recipes.Create, L("Permission:Recipes.Create"));
        recipes.AddChild(TastyBitesPermissions.Recipes.Edit, L("Permission:Recipes.Edit"));
        recipes.AddChild(TastyBitesPermissions.Recipes.Delete, L("Permission:Recipes.Delete"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<TastyBitesResource>(name);
    }
}
