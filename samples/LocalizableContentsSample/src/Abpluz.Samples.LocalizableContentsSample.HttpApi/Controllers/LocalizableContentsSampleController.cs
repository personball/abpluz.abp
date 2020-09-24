using Abpluz.Samples.LocalizableContentsSample.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace Abpluz.Samples.LocalizableContentsSample.Controllers
{
    /* Inherit your controllers from this class.
     */
    public abstract class LocalizableContentsSampleController : AbpController
    {
        protected LocalizableContentsSampleController()
        {
            LocalizationResource = typeof(LocalizableContentsSampleResource);
        }
    }
}