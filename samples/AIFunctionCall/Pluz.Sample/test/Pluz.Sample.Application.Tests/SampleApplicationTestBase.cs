using Volo.Abp.Modularity;

namespace Pluz.Sample;

public abstract class SampleApplicationTestBase<TStartupModule> : SampleTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
