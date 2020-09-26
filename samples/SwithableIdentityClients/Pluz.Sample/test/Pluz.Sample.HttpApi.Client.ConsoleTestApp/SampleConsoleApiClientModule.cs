using Abpluz.Abp.Http.Client.IdentityModel;
using Volo.Abp.Http.Client.IdentityModel;
using Volo.Abp.Modularity;

namespace Pluz.Sample.HttpApi.Client.ConsoleTestApp
{
    [DependsOn(
        typeof(SampleHttpApiClientModule),
        typeof(AbpHttpClientIdentityModelModule),
        typeof(PluzAbpHttpClientIdentityModelModule)
        )]
    public class SampleConsoleApiClientModule : AbpModule
    {

    }
}
