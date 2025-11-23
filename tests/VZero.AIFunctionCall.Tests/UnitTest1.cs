using System.Text.Json;
using System.Threading.Tasks;
using OpenAI.Chat;
using Shouldly;

namespace VZero.AIFunctionCall.Tests;

public class UnitTest1
{
    private readonly SampleAppService _sampleAppService;
    private readonly IFunctionCallExecutor _functionCallExecutor;
    public UnitTest1(
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
}
