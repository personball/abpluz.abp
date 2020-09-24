using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Volo.Abp;

namespace Abpluz.Samples.LocalizableContentsSample
{
    public class LocalizableContentsSampleWebTestStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplication<LocalizableContentsSampleWebTestModule>();
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            app.InitializeApplication();
        }
    }
}