using Volo.Abp.Http.Client.IdentityModel;
using Volo.Abp.Modularity;

namespace Abpluz.Samples.SwitchableIdentityClientsSample.HttpApi.Client.ConsoleTestApp
{
    [DependsOn(
        typeof(SwitchableIdentityClientsSampleHttpApiClientModule),
        typeof(AbpHttpClientIdentityModelModule)
        )]
    public class SwitchableIdentityClientsSampleConsoleApiClientModule : AbpModule
    {
        
    }
}
