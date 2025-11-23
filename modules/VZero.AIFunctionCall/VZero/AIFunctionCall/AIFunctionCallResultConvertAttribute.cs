namespace VZero.AIFunctionCall;

[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)] // 如果一个方法处理的类型一样，可以被多个 FunctionName 共用
public class AIFunctionCallResultConvertAttribute : Attribute
{
    public string FunctionName { get; }

    public AIFunctionCallResultConvertAttribute(string functionName)
    {
        FunctionName = functionName;
    }
}
