using System.Reflection;
using VZero.Abp.AIFunctionCall;

namespace Microsoft.Extensions.DependencyInjection;

public static class AIFunctionCallServiceCollectionExtensions
{
    public static ServiceCollection AddAIFunctionCalls(this ServiceCollection services, params Assembly[] assemblies)
    {
        foreach (var assembly in assemblies)
        {
            services.AddAIFunctionCall(assembly);
        }

        FunctionMetadataRegistry.MatchResultConvertMethodsToFunctions();

        return services;
    }

    internal static void AddAIFunctionCall(this ServiceCollection services, Assembly assembly)
    {
        // collect FunctionMetadata from assemblies
        var types = assembly.GetTypes()
                   .Where(t => t.IsClass && !t.IsAbstract)
                   .ToList();

        foreach (var type in types)
        {
            var convertMethods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
               .Where(m => m.GetCustomAttribute<AIFunctionCallResultConvertAttribute>() != null);
            foreach (var method in convertMethods)
            {
                var resultConvertAttributes = method.GetCustomAttributes<AIFunctionCallResultConvertAttribute>()!;
                foreach (var resultConvertAttribute in resultConvertAttributes)
                {
                    if (!FunctionMetadataRegistry.ResultConvertMethods.ContainsKey(resultConvertAttribute.FunctionName))
                    {
                        FunctionMetadataRegistry.ResultConvertMethods.TryAdd(resultConvertAttribute.FunctionName, (type, method));
                    }
                }
            }

            // 查找类中所有标记了AIFunctionCallAttribute的方法
            var functionMethods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                .Where(m => m.GetCustomAttribute<AIFunctionCallAttribute>() != null);
            foreach (var function in functionMethods)
            {
                var attributes = function.GetCustomAttributes<AIFunctionCallAttribute>()!;
                foreach (var attribute in attributes)
                {
                    if (!FunctionMetadataRegistry.Functions.ContainsKey(attribute.Name))
                    {
                        var metadata = new FunctionMetadata
                        {
                            Name = attribute.Name,
                            Description = attribute.Description,
                            MethodInfo = function,
                            ParametersSchema = function.GenerateSchema(),
                            ServiceType = type
                        };

                        // ResultConverterMethod
                        if (!attribute.ResultConverterMethodName.IsNullOrWhiteSpace())
                        {
                            var rcmi = type.GetMethod(attribute.ResultConverterMethodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
                            if (rcmi != null)
                            {
                                metadata.ResultConvertMethodInfo = rcmi;
                            }
                        }

                        FunctionMetadataRegistry.Functions.TryAdd(attribute.Name, metadata);
                    }
                }
            }
        }
    }
}
