using Abpluz.Samples.SwitchableIdentityClientsSample.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace Abpluz.Samples.SwitchableIdentityClientsSample
{
    [DependsOn(
        typeof(SwitchableIdentityClientsSampleEntityFrameworkCoreTestModule)
        )]
    public class SwitchableIdentityClientsSampleDomainTestModule : AbpModule
    {

    }
}