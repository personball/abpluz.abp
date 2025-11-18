namespace VZero.Abp.AIFunctionCall;

public class AIFunctionCallResultConvertAttribute : Attribute
{
    public string FunctionName { get; }

    public AIFunctionCallResultConvertAttribute(string functionName)
    {
        FunctionName = functionName;
    }
}
