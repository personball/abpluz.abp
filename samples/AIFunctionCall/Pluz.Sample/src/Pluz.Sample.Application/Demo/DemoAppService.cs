using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using OpenAI.Chat;
using Pluz.Sample.Demo.Dto;
using VZero.Abp.AIFunctionCall;

namespace Pluz.Sample.Demo;

public class DemoAppService : SampleAppService, IDemoAppService
{
    [AIFunctionCall(nameof(DoSthWithArgsAsync), "has input but no return")]
    public async Task DoSthWithArgsAsync([Description("this is an arg")] string arg)
    {
        Console.WriteLine($"[{nameof(DoSthWithArgsAsync)}] Called with input:{arg}");
    }

    [AIFunctionCall(nameof(DoSthWithoutArgsAsync), "无入参无返回值的示例")]
    public async Task DoSthWithoutArgsAsync()
    {
        Console.WriteLine($"[{nameof(DoSthWithoutArgsAsync)}] Called");
    }

    [AIFunctionCall(nameof(GetDemoDtoAsync), "将输入的name和value封装成一个DemoDto实例", nameof(ConvertDemoDtoToToolChatMessage))]
    public async Task<DemoDto> GetDemoDtoAsync(string name, int value)
    {
        return new DemoDto { Name = name, Value = value };
    }

    [AIFunctionCall(nameof(GetStringAsync), "重复a的值times次")]
    public async Task<string> GetStringAsync(string a, int times)
    {
        return string.Concat(Enumerable.Repeat(a, times));
    }

    // TODO: Convert Method 的签名验证规则
    public ToolChatMessage ConvertDemoDtoToToolChatMessage(DemoDto input, string functionName) // functionName 会自动赋值
    {
        return ChatMessage.CreateToolMessage(functionName, $"get a name:{input.Name} and value:{input.Value}");
    }
}