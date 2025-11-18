namespace VZero.Abp.AIFunctionCall;

[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public class AIFunctionCallAttribute : Attribute
{
    public string Name { get; }
    public string Description { get; }

    /// <summary>
    /// 结果要转换成 ChatMessage，需要关联一个方法，将原方法返回类型转成 ChatMessage;
    /// 如果没有特殊逻辑，正常返回结果可以有一个默认转换的 Handler（将结果序列化成json字符串返回给大模型）；
    /// 如果执行有异常，异常信息会统一封装返回
    /// </summary>
    public string? ResultConverterMethodName { get; }

    public AIFunctionCallAttribute(string name, string description, string? resultConverterMethodName = null)
    {
        Name = name;
        Description = description;
        ResultConverterMethodName = resultConverterMethodName;
    }
}
