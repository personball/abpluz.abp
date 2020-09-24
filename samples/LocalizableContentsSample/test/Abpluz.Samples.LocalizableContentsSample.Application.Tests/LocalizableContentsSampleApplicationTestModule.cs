using Volo.Abp.Modularity;

namespace Abpluz.Samples.LocalizableContentsSample
{
    [DependsOn(
        typeof(LocalizableContentsSampleApplicationModule),
        typeof(LocalizableContentsSampleDomainTestModule)
        )]
    public class LocalizableContentsSampleApplicationTestModule : AbpModule
    {

    }
}