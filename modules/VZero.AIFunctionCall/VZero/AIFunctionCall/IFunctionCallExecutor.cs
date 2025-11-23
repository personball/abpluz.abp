using OpenAI.Chat;

namespace VZero.AIFunctionCall;

public interface IFunctionCallExecutor
{
    /// <summary>
    /// 处理调用
    /// </summary>
    /// <param name="functionName"></param>
    /// <param name="functionArguments"></param>
    /// <returns></returns>
    Task<ToolChatMessage> ExecuteAsync(string functionName, BinaryData functionArguments);

    List<ChatTool> Tools { get; }
}