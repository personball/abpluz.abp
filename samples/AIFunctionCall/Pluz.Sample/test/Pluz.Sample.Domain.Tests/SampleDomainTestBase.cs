using Volo.Abp.Modularity;

namespace Pluz.Sample;

/* Inherit from this class for your domain layer tests. */
public abstract class SampleDomainTestBase<TStartupModule> : SampleTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
