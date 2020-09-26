using Pluz.Sample.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace Pluz.Sample
{
    [DependsOn(
        typeof(SampleEntityFrameworkCoreTestModule)
        )]
    public class SampleDomainTestModule : AbpModule
    {

    }
}