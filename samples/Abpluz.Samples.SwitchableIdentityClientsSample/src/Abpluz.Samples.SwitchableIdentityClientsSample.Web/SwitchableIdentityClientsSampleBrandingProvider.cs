using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared.Components;
using Volo.Abp.DependencyInjection;

namespace Abpluz.Samples.SwitchableIdentityClientsSample.Web
{
    [Dependency(ReplaceServices = true)]
    public class SwitchableIdentityClientsSampleBrandingProvider : DefaultBrandingProvider
    {
        public override string AppName => "SwitchableIdentityClientsSample";
    }
}
