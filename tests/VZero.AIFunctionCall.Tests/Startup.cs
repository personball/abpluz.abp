using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace VZero.AIFunctionCall.Tests;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddTransient<IDependency, DependencyClass>();
        services.AddTransient<SampleAppService>();

        services.AddAIFunctionCalls(Assembly.GetExecutingAssembly());
        services.AddTransient<IFunctionCallExecutor, DefaultFunctionCallExecutor>();
    }
}

public interface IDependency
{
    int Value { get; }
}

internal class DependencyClass : IDependency
{
    public int Value => 1;
}

public class MyAwesomeTests
{
    private readonly IDependency _d;

    public MyAwesomeTests(IDependency d) => _d = d;

    [Fact]
    public void AssertThatWeDoStuff()
    {
        Assert.Equal(1, _d.Value);
    }
}