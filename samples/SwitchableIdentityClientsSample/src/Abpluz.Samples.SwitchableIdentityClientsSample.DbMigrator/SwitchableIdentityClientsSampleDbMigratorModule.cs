using Abpluz.Samples.SwitchableIdentityClientsSample.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Modularity;

namespace Abpluz.Samples.SwitchableIdentityClientsSample.DbMigrator
{
    [DependsOn(
        typeof(AbpAutofacModule),
        typeof(SwitchableIdentityClientsSampleEntityFrameworkCoreDbMigrationsModule),
        typeof(SwitchableIdentityClientsSampleApplicationContractsModule)
        )]
    public class SwitchableIdentityClientsSampleDbMigratorModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpBackgroundJobOptions>(options => options.IsJobExecutionEnabled = false);
        }
    }
}
