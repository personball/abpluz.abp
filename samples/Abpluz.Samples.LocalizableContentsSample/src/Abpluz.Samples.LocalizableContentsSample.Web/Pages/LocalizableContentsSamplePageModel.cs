using Abpluz.Samples.LocalizableContentsSample.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Abpluz.Samples.LocalizableContentsSample.Web.Pages
{
    /* Inherit your PageModel classes from this class.
     */
    public abstract class LocalizableContentsSamplePageModel : AbpPageModel
    {
        protected LocalizableContentsSamplePageModel()
        {
            LocalizationResourceType = typeof(LocalizableContentsSampleResource);
        }
    }
}