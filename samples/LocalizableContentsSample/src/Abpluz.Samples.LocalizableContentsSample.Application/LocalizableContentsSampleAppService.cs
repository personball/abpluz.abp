using System;
using System.Collections.Generic;
using System.Text;
using Abpluz.Samples.LocalizableContentsSample.Localization;
using Volo.Abp.Application.Services;

namespace Abpluz.Samples.LocalizableContentsSample
{
    /* Inherit your application services from this class.
     */
    public abstract class LocalizableContentsSampleAppService : ApplicationService
    {
        protected LocalizableContentsSampleAppService()
        {
            LocalizationResource = typeof(LocalizableContentsSampleResource);
        }
    }
}
