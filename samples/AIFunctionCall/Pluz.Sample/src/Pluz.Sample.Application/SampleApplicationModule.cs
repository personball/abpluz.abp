using Volo.Abp.Account;
using Volo.Abp.AutoMapper;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.TenantManagement;
using VZero.Abp.AIFunctionCall;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Pluz.Sample;

[DependsOn(
    typeof(SampleDomainModule),
    typeof(AbpAccountApplicationModule),
    typeof(SampleApplicationContractsModule),
    typeof(AbpIdentityApplicationModule),
    typeof(AbpPermissionManagementApplicationModule),
    typeof(AbpTenantManagementApplicationModule),
    typeof(AbpFeatureManagementApplicationModule),
    typeof(AbpSettingManagementApplicationModule),
    typeof(AIFunctionCallModule)
    )]
public class SampleApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<SampleApplicationModule>();
        });

        context.Services.AddAIFunctionCalls(Assembly.GetExecutingAssembly());

    }
}
