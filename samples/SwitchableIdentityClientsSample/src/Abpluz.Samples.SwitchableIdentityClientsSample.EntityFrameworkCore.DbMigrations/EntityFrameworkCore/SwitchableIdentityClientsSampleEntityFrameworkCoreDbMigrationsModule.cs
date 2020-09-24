using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace Abpluz.Samples.SwitchableIdentityClientsSample.EntityFrameworkCore
{
    [DependsOn(
        typeof(SwitchableIdentityClientsSampleEntityFrameworkCoreModule)
        )]
    public class SwitchableIdentityClientsSampleEntityFrameworkCoreDbMigrationsModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddAbpDbContext<SwitchableIdentityClientsSampleMigrationsDbContext>();
        }
    }
}
