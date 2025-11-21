using System.Reflection;
using Microsoft.Extensions.Logging;
using OpenAI.Chat;
using VZero.Abp.AIFunctionCall;

namespace Microsoft.Extensions.DependencyInjection;

public static class AIFunctionCallServiceCollectionExtensions
{
    public static IServiceCollection AddAIFunctionCalls(this IServiceCollection services, params Assembly[] assemblies)
    {
        foreach (var assembly in assemblies)
        {
            services.AddAIFunctionCall(assembly);
        }

        FunctionMetadataRegistry.MatchResultConvertMethodsToFunctions();

        return services;
    }
    internal static bool IsTaskOrTaskValueToolChatMessage(this Type type)
    {
        var isGenericType = type.IsGenericType;
        var isGenericTask = type.GetGenericTypeDefinition() == typeof(Task<>);
        var isGenericValueTask = type.GetGenericTypeDefinition() == typeof(ValueTask<>);
        var isToolChatMessage = type.GetGenericArguments().Contains(typeof(ToolChatMessage));
        return isGenericType && isToolChatMessage && (isGenericTask || isGenericValueTask);
    }


    internal static void AddAIFunctionCall(this IServiceCollection services, Assembly assembly)
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
                                if (rcmi.ReturnType == typeof(ToolChatMessage) || type.IsTaskOrTaskValueToolChatMessage())
                                {
                                    metadata.ResultConvertMethodInfo = rcmi;
                                }
                                else
                                {
                                    var logger = services.BuildServiceProvider().GetRequiredService<ILogger<AIFunctionCallModule>>();
                                    logger.LogWarning($"The method '{attribute.ResultConverterMethodName}' in type '{type.FullName}' does not return ToolChatMessage or Task/ValueTask of ToolChatMessage.");
                                }
                            }
                        }

                        FunctionMetadataRegistry.Functions.TryAdd(attribute.Name, metadata);
                    }
                }
            }
        }
    }
}
