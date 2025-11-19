namespace VZero.Abp.AIFunctionCall;

[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)] // 多个 Function 实际上是同一个方法好像也没关系
public class AIFunctionCallAttribute : Attribute
{
    /// <summary>
    /// Function Name
    /// </summary>
    public string Name { get; }
    /// <summary>
    /// Function Description
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// 结果要转换成 ChatMessage，需要关联一个方法，将原方法返回类型转成 ChatMessage;
    /// 如果没有特殊逻辑，正常返回结果可以有一个默认转换的 Handler:
    /// 1.如果结果是一个 ChatMessage 则直接返回;
    /// 2.如果结果不是 ChatMessage, 则将结果序列化成json字符串包在一个 ChatMessage 中返回；
    /// 如果执行有异常，异常信息会统一封装返回
    /// </summary>
    public string? ResultConverterMethodName { get; }

    /// <summary>
    /// 标注指定方法作为给LLM使用的 FunctionCall
    /// </summary>
    /// <param name="name">FunctionName</param>
    /// <param name="description">告知 LLM 此 Function 的功能描述</param>
    /// <param name="resultConverterMethodName">结果转换逻辑方法名，只有和 FunctionCall 在同一个类型上时才使用</param>
    public AIFunctionCallAttribute(string name, string description, string? resultConverterMethodName = null)
    {
        Name = name;
        Description = description;
        ResultConverterMethodName = resultConverterMethodName;
    }
}
