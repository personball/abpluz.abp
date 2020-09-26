using System;
using System.Collections.Generic;
using System.Text;
using Pluz.Sample.Localization;
using Volo.Abp.Application.Services;

namespace Pluz.Sample
{
    /* Inherit your application services from this class.
     */
    public abstract class SampleAppService : ApplicationService
    {
        protected SampleAppService()
        {
            LocalizationResource = typeof(SampleResource);
        }
    }
}
