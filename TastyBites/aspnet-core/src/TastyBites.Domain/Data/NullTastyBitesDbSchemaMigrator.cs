using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace TastyBites.Data;

/* This is used if database provider does't define
 * ITastyBitesDbSchemaMigrator implementation.
 */
public class NullTastyBitesDbSchemaMigrator : ITastyBitesDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
