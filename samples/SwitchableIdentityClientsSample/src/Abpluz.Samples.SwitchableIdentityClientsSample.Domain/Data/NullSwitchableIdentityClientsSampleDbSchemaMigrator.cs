using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Abpluz.Samples.SwitchableIdentityClientsSample.Data
{
    /* This is used if database provider does't define
     * ISwitchableIdentityClientsSampleDbSchemaMigrator implementation.
     */
    public class NullSwitchableIdentityClientsSampleDbSchemaMigrator : ISwitchableIdentityClientsSampleDbSchemaMigrator, ITransientDependency
    {
        public Task MigrateAsync()
        {
            return Task.CompletedTask;
        }
    }
}