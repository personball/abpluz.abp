using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenAI.Chat;

namespace VZero.Abp.AIFunctionCall;

public class DefaultFunctionCallExecutor : IFunctionCallExecutor
{
    private static readonly List<ChatTool> _tools = new List<ChatTool>();
    private ILogger<DefaultFunctionCallExecutor> _logger;
    private IServiceProvider _serviceProvider;

    public List<ChatTool> Tools
    {
        get
        {
            if (_tools.Any())
            {
                return _tools;
            }

            BuildChatTools(_tools);

            return _tools;
        }
    }

    public DefaultFunctionCallExecutor(
        ILogger<DefaultFunctionCallExecutor> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public async Task<ToolChatMessage> ExecuteAsync(string functionName, BinaryData functionArguments)
    {
        // 正常结果以 Ok 返回，记录 info 级别日志
        ToolChatMessage Ok(string content)
        {
            _logger.LogInformation($"[{functionName}] {content}");
            return ChatMessage.CreateToolMessage(functionName, content);
        }

        // LLM 侧提供的内容引发的问题用 Warn 返回，记录 Warn 级别的日志
        ToolChatMessage Warn(string content, string? detail = null)
        {
            _logger.LogWarning($"[{functionName}] {content}, Detail:{detail}");
            return ChatMessage.CreateToolMessage(functionName, content);
        }

        // 本地引发的问题用 Error 返回记录 Error 级别日志
        ToolChatMessage Error(string content, string? detail = null)
        {
            _logger.LogError($"[{functionName}] {content}, Detail:{detail}");
            return ChatMessage.CreateToolMessage(functionName, content);
        }

        // 获取对应的 metadata
        if (!FunctionMetadataRegistry.Functions.TryGetValue(functionName, out FunctionMetadata? metadata))
        {
            return Error($"{functionName} Not Found!");
        }

        try
        {
            // 实例化 service 
            var service = _serviceProvider.GetRequiredService(metadata.ServiceType);
            if (service == null)
            {
                return Error($"Create instance for {metadata.ServiceType.FullName} Failed!");
            }

            // 准备参数
            var jsonStr = functionArguments.ToString();
            var paramsJsonNode = JsonNode.Parse(jsonStr);
            if (paramsJsonNode == null)
            {
                return Warn("Failed to parse function arguments!", jsonStr);
            }

            // 整体校验
            var validationResults = metadata.ParametersSchema.Evaluate(paramsJsonNode);
            if (!validationResults.IsValid)
            {
                if (validationResults.HasErrors)
                    return Warn("Function Arguments Not Valid: " + string.Join(",", validationResults.Errors!.Select(e => $" the {e.Key} {e.Value}")));
                return Warn("Function Arguments Not Valid!");
            }

            // 第一层是封包的object，需要拆开
            var parametersJson = paramsJsonNode!.AsObject()
                .ToDictionary(p => p.Key, p => p.Value?.ToJsonString());
            // 基于 MethodInfo 依次提取 parameter
            var methodParameters = metadata.MethodInfo.GetParameters();
            // 验证参数个数是否匹配
            if (parametersJson.Count != methodParameters.Length)
            {
                return Warn($"Function Arguments Count Mismatch! Provided: {parametersJson.Count}, Required: {methodParameters.Length}");
            }

            // 验证参数名是否齐全
            var missingParameters = methodParameters
                .Where(param => !parametersJson.ContainsKey(param.Name!))
                .Select(param => param.Name)
                .ToList();

            if (missingParameters.Any())
            {
                return Warn("Missing Required Parameters: " + string.Join(", ", missingParameters));
            }

            var deserializedParams = methodParameters.Select(param =>
                JsonSerializer.Deserialize(parametersJson[param.Name!]!, param.ParameterType, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    ReadCommentHandling = JsonCommentHandling.Skip,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
                })).ToArray();

            // 调用
            // 是否需要手动开 uow ？检查Session？检查TenantId？
            object? result;
            if (metadata.MethodInfo.ReturnType == typeof(void))
            {
                metadata.MethodInfo.Invoke(service, deserializedParams);
                result = null;
            }
            else if (metadata.MethodInfo.ReturnType == typeof(Task))
            {
                await (Task)metadata.MethodInfo.Invoke(service, deserializedParams)!;
                result = null;
            }
            else if (metadata.MethodInfo.ReturnType.IsGenericType && metadata.MethodInfo.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
            {
                var task = (Task)metadata.MethodInfo.Invoke(service, deserializedParams)!;
                await task.ConfigureAwait(false);
                result = task.GetType().GetProperty("Result")?.GetValue(task);
            }
            else if (metadata.MethodInfo.ReturnType == typeof(ValueTask))
            {
                await (ValueTask)metadata.MethodInfo.Invoke(service, deserializedParams)!;
                result = null;
            }
            else if (metadata.MethodInfo.ReturnType.IsGenericType && metadata.MethodInfo.ReturnType.GetGenericTypeDefinition() == typeof(ValueTask<>))
            {
                var valueTask = (ValueTask)metadata.MethodInfo.Invoke(service, deserializedParams)!;
                await valueTask.ConfigureAwait(false);
                result = metadata.MethodInfo.ReturnType.GetProperty("Result")?.GetValue(valueTask);
            }
            else
            {
                result = metadata.MethodInfo.Invoke(service, deserializedParams);
            }

            if (result == null)
            {
                // 无返回值的情况
                return Ok("Done");
            }

            if (metadata.ResultConvertMethodInfo == null)
            {
                // 默认返回逻辑
                if (result is ToolChatMessage toolChatMessage)
                {
                    _logger.LogInformation($"[{functionName}]{toolChatMessage.Content}");
                    return toolChatMessage;
                }

                return Ok(JsonSerializer.Serialize(result));
            }

            // 结果转换
            if (metadata.ResultConvertServiceType == null)
            {
                var message = metadata.ResultConvertMethodInfo.Invoke(service, [result, metadata.Name]);
                if (message == null)
                {
                    return Error(
                        $"Result Convert into null, not expected! result:{JsonSerializer.Serialize(result)}",
                        $"ResultConvertMethod:{metadata.ResultConvertMethodInfo.Name} on Type:{metadata.ServiceType.FullName}"
                    );
                }

                if (message is ToolChatMessage toolChatMessage)
                {
                    _logger.LogInformation($"[{functionName}]{toolChatMessage.Content}");
                    return toolChatMessage;
                }

                return Error(
                    $"Result Convert into {message.GetType().FullName}, not expected! result:{JsonSerializer.Serialize(result)}",
                    $"ResultConvertMethod:{metadata.ResultConvertMethodInfo.Name} on Type:{metadata.ServiceType.FullName}"
                );
            }

            var converterService = _serviceProvider.GetRequiredService(metadata.ResultConvertServiceType!);
            if (converterService != null)
            {
                var message = metadata.ResultConvertMethodInfo.Invoke(converterService, [result, metadata.Name]);
                if (message == null)
                {
                    return Error(
                        $"Result Convert into null, not expected! result:{JsonSerializer.Serialize(result)}",
                        $"ResultConvertMethod:{metadata.ResultConvertMethodInfo.Name} on Type:{metadata.ResultConvertServiceType.FullName}"
                    );
                }

                if (message is ToolChatMessage toolChatMessage)
                {
                    _logger.LogInformation($"[{functionName}]{toolChatMessage.Content}");
                    return toolChatMessage;
                }

                return Error(
                    $"Result Convert into {message.GetType().FullName}, not expected! result:{JsonSerializer.Serialize(result)}",
                    $"ResultConvertMethod:{metadata.ResultConvertMethodInfo.Name} on Type:{metadata.ResultConvertServiceType.FullName}"
                );
            }
            else
            {
                return Error($"Create instance for {metadata.ResultConvertServiceType.FullName} Failed!");
            }
        }
        catch (Exception ex)
        {
            // 异常处理
            _logger.LogError(ex, ex.Message);
            return ChatMessage.CreateToolMessage(functionName, $"Something wrong:{ex.Message}");
        }
    }

    private void BuildChatTools([NotNull] List<ChatTool> tools)
    {
        if (tools.Any())
            tools.Clear();

        var functions = FunctionMetadataRegistry.Functions;
        foreach (var function in functions)
        {
            var tool = ChatTool.CreateFunctionTool(
                functionName: function.Value.Name,
                 functionDescription: function.Value.Description,
                 functionParameters: BinaryData.FromString(JsonSerializer.Serialize(function.Value.ParametersSchema))
            );

            tools.Add(tool);
        }
    }
}
