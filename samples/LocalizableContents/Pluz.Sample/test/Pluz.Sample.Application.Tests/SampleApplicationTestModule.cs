using Volo.Abp.Modularity;

namespace Pluz.Sample
{
    [DependsOn(
        typeof(SampleApplicationModule),
        typeof(SampleDomainTestModule)
        )]
    public class SampleApplicationTestModule : AbpModule
    {

    }
}