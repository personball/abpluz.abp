using Abpluz.Samples.SwitchableIdentityClientsSample.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace Abpluz.Samples.SwitchableIdentityClientsSample.Controllers
{
    /* Inherit your controllers from this class.
     */
    public abstract class SwitchableIdentityClientsSampleController : AbpController
    {
        protected SwitchableIdentityClientsSampleController()
        {
            LocalizationResource = typeof(SwitchableIdentityClientsSampleResource);
        }
    }
}