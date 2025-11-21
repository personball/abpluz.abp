using Pluz.Sample.Demo;
using Xunit;

namespace Pluz.Sample.EntityFrameworkCore.Applications;

[Collection(SampleTestConsts.CollectionDefinitionName)]
public class EfCoreDemoAppServiceTests : DemoAppServiceTests<SampleEntityFrameworkCoreTestModule>
{

}
