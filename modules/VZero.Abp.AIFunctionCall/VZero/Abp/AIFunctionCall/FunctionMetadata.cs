using System.Reflection;
using Json.Schema;

namespace VZero.Abp.AIFunctionCall;

public class FunctionMetadata
{
    // AIFunctionCallAttribute Name
    public string Name { get; set; } = string.Empty;

    // AIFunctionCallAttribute Description
    public string Description { get; set; } = string.Empty;

    // 标记了 AIFunctionCallAttribute 的方法所属的类型，需要注册ioc
    public Type ServiceType { get; set; } = null!;

    // 标记了 AIFunctionCallAttribute 的方法
    public MethodInfo MethodInfo { get; set; } = null!;

    // 针对所有入参封装 schema，以参数名作为 properties
    public JsonSchema ParametersSchema { get; set; } = null!;

    // 结果转换逻辑所在的类型，如果为空，默认转换方法在 ServiceType 上 
    // 必须用 AIFunctionCallResultConvertAttribute 标注
    public Type? ResultConvertServiceType { get; set; }

    // 结果转换方法
    public MethodInfo? ResultConvertMethodInfo { get; set; }
}
