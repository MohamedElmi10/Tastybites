using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TastyBites.Data;
using Volo.Abp.DependencyInjection;

namespace TastyBites.EntityFrameworkCore;

public class EntityFrameworkCoreTastyBitesDbSchemaMigrator
    : ITastyBitesDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCoreTastyBitesDbSchemaMigrator(
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolve the TastyBitesDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<TastyBitesDbContext>()
            .Database
            .MigrateAsync();
    }
}
