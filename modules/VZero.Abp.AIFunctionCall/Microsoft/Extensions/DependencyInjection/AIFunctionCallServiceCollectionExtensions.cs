using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection;

public static class AIFunctionCallServiceCollectionExtensions
{
    public static void AddAIFunctionCall(this ServiceCollection services, params Assembly[] assemblies)
    {
        // collect FunctionMetadata from assemblies

    }
}
