using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Abpluz.Samples.LocalizableContentsSample.Data
{
    /* This is used if database provider does't define
     * ILocalizableContentsSampleDbSchemaMigrator implementation.
     */
    public class NullLocalizableContentsSampleDbSchemaMigrator : ILocalizableContentsSampleDbSchemaMigrator, ITransientDependency
    {
        public Task MigrateAsync()
        {
            return Task.CompletedTask;
        }
    }
}