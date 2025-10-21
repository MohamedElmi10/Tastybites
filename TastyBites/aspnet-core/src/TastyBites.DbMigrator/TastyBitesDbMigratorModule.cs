using TastyBites.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace TastyBites.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(TastyBitesEntityFrameworkCoreModule),
    typeof(TastyBitesApplicationContractsModule)
    )]
public class TastyBitesDbMigratorModule : AbpModule
{
}
