using TastyBites.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace TastyBites;

[DependsOn(
    typeof(TastyBitesEntityFrameworkCoreTestModule)
    )]
public class TastyBitesDomainTestModule : AbpModule
{

}
