using Volo.Abp.Modularity;

namespace Abpluz.Samples.SwitchableIdentityClientsSample
{
    [DependsOn(
        typeof(SwitchableIdentityClientsSampleApplicationModule),
        typeof(SwitchableIdentityClientsSampleDomainTestModule)
        )]
    public class SwitchableIdentityClientsSampleApplicationTestModule : AbpModule
    {

    }
}