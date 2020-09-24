using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace Abpluz.Samples.LocalizableContentsSample.EntityFrameworkCore
{
    [DependsOn(
        typeof(LocalizableContentsSampleEntityFrameworkCoreModule)
        )]
    public class LocalizableContentsSampleEntityFrameworkCoreDbMigrationsModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddAbpDbContext<LocalizableContentsSampleMigrationsDbContext>();
        }
    }
}
