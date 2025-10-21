using Volo.Abp.Modularity;

namespace TastyBites;

[DependsOn(
    typeof(TastyBitesApplicationModule),
    typeof(TastyBitesDomainTestModule)
    )]
public class TastyBitesApplicationTestModule : AbpModule
{

}
