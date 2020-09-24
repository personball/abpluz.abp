using System;
using System.Collections.Generic;
using System.Text;
using Abpluz.Samples.SwitchableIdentityClientsSample.Localization;
using Volo.Abp.Application.Services;

namespace Abpluz.Samples.SwitchableIdentityClientsSample
{
    /* Inherit your application services from this class.
     */
    public abstract class SwitchableIdentityClientsSampleAppService : ApplicationService
    {
        protected SwitchableIdentityClientsSampleAppService()
        {
            LocalizationResource = typeof(SwitchableIdentityClientsSampleResource);
        }
    }
}
