using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace VZero.AIFunctionCall.Tests;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // services.AddTransient<IDependency, DependencyClass>();

        services.AddTransient<SampleAppService>();

        // setup AIFunctionCall
        services.AddAIFunctionCalls(Assembly.GetExecutingAssembly());
        services.AddTransient<IFunctionCallExecutor, DefaultFunctionCallExecutor>();
    }
}