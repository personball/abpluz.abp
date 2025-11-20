using Pluz.Sample.Samples;
using Xunit;

namespace Pluz.Sample.EntityFrameworkCore.Applications;

[Collection(SampleTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<SampleEntityFrameworkCoreTestModule>
{

}
