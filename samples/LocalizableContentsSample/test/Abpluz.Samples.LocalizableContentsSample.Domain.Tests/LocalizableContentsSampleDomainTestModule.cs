using Abpluz.Samples.LocalizableContentsSample.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace Abpluz.Samples.LocalizableContentsSample
{
    [DependsOn(
        typeof(LocalizableContentsSampleEntityFrameworkCoreTestModule)
        )]
    public class LocalizableContentsSampleDomainTestModule : AbpModule
    {

    }
}