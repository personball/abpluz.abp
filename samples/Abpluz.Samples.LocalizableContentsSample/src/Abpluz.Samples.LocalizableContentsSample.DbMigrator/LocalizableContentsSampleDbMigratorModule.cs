using Abpluz.Samples.LocalizableContentsSample.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Modularity;

namespace Abpluz.Samples.LocalizableContentsSample.DbMigrator
{
    [DependsOn(
        typeof(AbpAutofacModule),
        typeof(LocalizableContentsSampleEntityFrameworkCoreDbMigrationsModule),
        typeof(LocalizableContentsSampleApplicationContractsModule)
        )]
    public class LocalizableContentsSampleDbMigratorModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpBackgroundJobOptions>(options => options.IsJobExecutionEnabled = false);
        }
    }
}
