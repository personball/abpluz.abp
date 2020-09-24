using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Abpluz.Samples.LocalizableContentsSample.Data;
using Volo.Abp.DependencyInjection;

namespace Abpluz.Samples.LocalizableContentsSample.EntityFrameworkCore
{
    public class EntityFrameworkCoreLocalizableContentsSampleDbSchemaMigrator
        : ILocalizableContentsSampleDbSchemaMigrator, ITransientDependency
    {
        private readonly IServiceProvider _serviceProvider;

        public EntityFrameworkCoreLocalizableContentsSampleDbSchemaMigrator(
            IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task MigrateAsync()
        {
            /* We intentionally resolving the LocalizableContentsSampleMigrationsDbContext
             * from IServiceProvider (instead of directly injecting it)
             * to properly get the connection string of the current tenant in the
             * current scope.
             */

            await _serviceProvider
                .GetRequiredService<LocalizableContentsSampleMigrationsDbContext>()
                .Database
                .MigrateAsync();
        }
    }
}