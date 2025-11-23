using System.Text.Json;
using System.Threading.Tasks;
using OpenAI.Chat;
using Shouldly;

namespace VZero.AIFunctionCall.Tests;

public class AIFunctionCallTest1
{
    private readonly SampleAppService _sampleAppService;
    private readonly IFunctionCallExecutor _functionCallExecutor;
    public AIFunctionCallTest1(
        SampleAppService sampleAppService,
        IFunctionCallExecutor functionCallExecutor)
    {
        _sampleAppService = sampleAppService;
        _functionCallExecutor = functionCallExecutor;
    }

    [Fact]
    public async Task Call_GetStringAsync_Directly()
    {
        var str = await _sampleAppService.GetStringAsync();
        str.ShouldBe("123");
    }

    [Fact]
    public async Task Call_GetStringAsync_By_Executor()
    {
        var message = await _functionCallExecutor.ExecuteAsync(nameof(_sampleAppService.GetStringAsync), BinaryData.FromString("{}"));

        message.ToJsonString().ShouldBe(ChatMessage.CreateToolMessage(nameof(_sampleAppService.GetStringAsync), "123").ToJsonString());
    }

    [Fact]
    public async Task Tools_Count()
    {
        var tools = _functionCallExecutor.Tools;
        tools.Count.ShouldBe(1);
    }
}
