using Pluz.Sample.Samples;
using Xunit;

namespace Pluz.Sample.EntityFrameworkCore.Domains;

[Collection(SampleTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<SampleEntityFrameworkCoreTestModule>
{

}
