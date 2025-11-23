using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;
using VZero.AIFunctionCall;

namespace VZero.Abp.AIFunctionCall;

public class AIFunctionCallModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        // 注册 IFunctionCallExecutor 实现
        context.Services.AddTransient<IFunctionCallExecutor, DefaultFunctionCallExecutor>();
    }
}