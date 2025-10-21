using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace TastyBites;

[Dependency(ReplaceServices = true)]
public class TastyBitesBrandingProvider : DefaultBrandingProvider
{
    public override string AppName => "TastyBites";
}
