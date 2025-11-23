using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using OpenAI.Chat;
using Pluz.Sample.Demo.Dto;
using VZero.AIFunctionCall;

namespace Pluz.Sample.Demo;

public class DemoConvertAppService : SampleAppService
{
    [AIFunctionCallResultConvert("GetDemoDtoAsync")]
    public ToolChatMessage ConvertDemoDtoToToolChatMessage(DemoDto input, string functionName) // functionName 会自动赋值
    {
        return ChatMessage.CreateToolMessage(functionName, $"get a name:{input.Name} and value:{input.Value}");
    }
}