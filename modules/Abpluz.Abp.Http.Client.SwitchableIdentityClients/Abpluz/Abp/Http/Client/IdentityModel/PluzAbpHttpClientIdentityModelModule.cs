using Volo.Abp.Http.Client;
using Volo.Abp.IdentityModel;
using Volo.Abp.Modularity;

namespace Abpluz.Abp.Http.Client.IdentityModel
{
    [DependsOn(
        typeof(AbpHttpClientModule),
        typeof(AbpIdentityModelModule))]
    public class PluzAbpHttpClientIdentityModelModule : AbpModule
    {
    }
}
