using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Abpluz.Samples.SwitchableIdentityClientsSample.Data;
using Volo.Abp.DependencyInjection;

namespace Abpluz.Samples.SwitchableIdentityClientsSample.EntityFrameworkCore
{
    public class EntityFrameworkCoreSwitchableIdentityClientsSampleDbSchemaMigrator
        : ISwitchableIdentityClientsSampleDbSchemaMigrator, ITransientDependency
    {
        private readonly IServiceProvider _serviceProvider;

        public EntityFrameworkCoreSwitchableIdentityClientsSampleDbSchemaMigrator(
            IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task MigrateAsync()
        {
            /* We intentionally resolving the SwitchableIdentityClientsSampleMigrationsDbContext
             * from IServiceProvider (instead of directly injecting it)
             * to properly get the connection string of the current tenant in the
             * current scope.
             */

            await _serviceProvider
                .GetRequiredService<SwitchableIdentityClientsSampleMigrationsDbContext>()
                .Database
                .MigrateAsync();
        }
    }
}