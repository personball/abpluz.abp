using Volo.Abp.Http.Client.IdentityModel;
using Volo.Abp.Modularity;

namespace Abpluz.Samples.LocalizableContentsSample.HttpApi.Client.ConsoleTestApp
{
    [DependsOn(
        typeof(LocalizableContentsSampleHttpApiClientModule),
        typeof(AbpHttpClientIdentityModelModule)
        )]
    public class LocalizableContentsSampleConsoleApiClientModule : AbpModule
    {
        
    }
}
