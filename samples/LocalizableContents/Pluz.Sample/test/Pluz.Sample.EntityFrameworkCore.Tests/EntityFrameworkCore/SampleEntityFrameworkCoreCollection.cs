using Xunit;

namespace Pluz.Sample.EntityFrameworkCore
{
    [CollectionDefinition(SampleTestConsts.CollectionDefinitionName)]
    public class SampleEntityFrameworkCoreCollection : ICollectionFixture<SampleEntityFrameworkCoreFixture>
    {

    }
}
