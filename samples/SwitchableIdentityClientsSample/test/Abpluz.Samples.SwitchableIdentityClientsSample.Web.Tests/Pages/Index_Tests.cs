using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace Abpluz.Samples.SwitchableIdentityClientsSample.Pages
{
    [Collection(SwitchableIdentityClientsSampleTestConsts.CollectionDefinitionName)]
    public class Index_Tests : SwitchableIdentityClientsSampleWebTestBase
    {
        [Fact]
        public async Task Welcome_Page()
        {
            var response = await GetResponseAsStringAsync("/");
            response.ShouldNotBeNull();
        }
    }
}
