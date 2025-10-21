using System.Threading.Tasks;

namespace TastyBites.Data;

public interface ITastyBitesDbSchemaMigrator
{
    Task MigrateAsync();
}
