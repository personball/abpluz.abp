using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace Abpluz.Samples.LocalizableContentsSample.Pages
{
    [Collection(LocalizableContentsSampleTestConsts.CollectionDefinitionName)]
    public class Index_Tests : LocalizableContentsSampleWebTestBase
    {
        [Fact]
        public async Task Welcome_Page()
        {
            var response = await GetResponseAsStringAsync("/");
            response.ShouldNotBeNull();
        }
    }
}
