using Xunit;

namespace Abpluz.Samples.LocalizableContentsSample.EntityFrameworkCore
{
    [CollectionDefinition(LocalizableContentsSampleTestConsts.CollectionDefinitionName)]
    public class LocalizableContentsSampleEntityFrameworkCoreCollection : ICollectionFixture<LocalizableContentsSampleEntityFrameworkCoreFixture>
    {

    }
}
