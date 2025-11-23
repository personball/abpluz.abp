using System.Text.Json;
using OpenAI.Chat;

namespace VZero.AIFunctionCall.Tests;

public static class ChatMessageBase
{
    public static string ToJsonString(this ToolChatMessage toolChatMessage)
    {
        return JsonSerializer.Serialize(toolChatMessage);
    }
}