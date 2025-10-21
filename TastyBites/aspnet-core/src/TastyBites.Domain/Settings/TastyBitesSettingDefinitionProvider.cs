using Volo.Abp.Settings;

namespace TastyBites.Settings;

public class TastyBitesSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(TastyBitesSettings.MySetting1));
    }
}
