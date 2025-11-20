using Volo.Abp.Modularity;

namespace Pluz.Sample;

[DependsOn(
    typeof(SampleDomainModule),
    typeof(SampleTestBaseModule)
)]
public class SampleDomainTestModule : AbpModule
{

}
