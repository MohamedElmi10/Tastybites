using TastyBites.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace TastyBites.Permissions;

public class TastyBitesPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(TastyBitesPermissions.GroupName);
        //Define your own permissions here. Example:
        //myGroup.AddPermission(TastyBitesPermissions.MyPermission1, L("Permission:MyPermission1"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<TastyBitesResource>(name);
    }
}
