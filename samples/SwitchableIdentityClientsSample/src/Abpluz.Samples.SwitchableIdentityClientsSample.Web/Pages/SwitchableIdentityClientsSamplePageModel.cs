using Abpluz.Samples.SwitchableIdentityClientsSample.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Abpluz.Samples.SwitchableIdentityClientsSample.Web.Pages
{
    /* Inherit your PageModel classes from this class.
     */
    public abstract class SwitchableIdentityClientsSamplePageModel : AbpPageModel
    {
        protected SwitchableIdentityClientsSamplePageModel()
        {
            LocalizationResourceType = typeof(SwitchableIdentityClientsSampleResource);
        }
    }
}