using OpenAI.Chat;

namespace VZero.Abp.AIFunctionCall;

public interface IFunctionCallExecutor
{
    /// <summary>
    /// 处理调用
    /// </summary>
    /// <param name="functionName"></param>
    /// <param name="functionArguments"></param>
    /// <returns></returns>
    Task<ChatMessage> HandleAsync(string functionName, BinaryData functionArguments);
}