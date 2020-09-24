using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared.Components;
using Volo.Abp.DependencyInjection;

namespace Abpluz.Samples.LocalizableContentsSample.Web
{
    [Dependency(ReplaceServices = true)]
    public class LocalizableContentsSampleBrandingProvider : DefaultBrandingProvider
    {
        public override string AppName => "LocalizableContentsSample";
    }
}
