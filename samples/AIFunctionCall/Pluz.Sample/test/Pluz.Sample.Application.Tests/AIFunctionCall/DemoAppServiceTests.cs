using Pluz.Sample.Samples;
using Shouldly;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using VZero.Abp.AIFunctionCall;
using Xunit;

namespace Pluz.Sample.Demo;

public abstract class DemoAppServiceTests<TStartupModule> : SampleApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly IDemoAppService _demoAppService;
    private readonly IFunctionCallExecutor _functionCallExecutor;

    public DemoAppServiceTests()
    {
        _demoAppService = GetRequiredService<IDemoAppService>();
        _functionCallExecutor = GetRequiredService<IFunctionCallExecutor>();
    }

    [Fact]
    public async Task Tools_Should_OK()
    {
        var tools = _functionCallExecutor.Tools;

        tools.Count.ShouldBe(4);

    }

    [Fact]
    public async Task FunctionCallExecutor_Should_Work()
    {
        //Act
        await _demoAppService.DoSthWithArgsAsync("123");
        await _demoAppService.DoSthWithoutArgsAsync();

        var str = await _demoAppService.GetStringAsync("hello", 3);
        var dto = await _demoAppService.GetDemoDtoAsync("bike", 99);
        var dtoJson = JsonSerializer.Serialize(dto);
        // Act by FunctionCallExecutor
        await _functionCallExecutor.ExecuteAsync(nameof(_demoAppService.DoSthWithArgsAsync), BinaryData.FromString("""{"arg":"123"}"""));
        await _functionCallExecutor.ExecuteAsync(nameof(_demoAppService.DoSthWithoutArgsAsync), BinaryData.FromString("{}"));

        var str2 = await _functionCallExecutor.ExecuteAsync(nameof(_demoAppService.GetStringAsync), BinaryData.FromString("""{"a":"hello","times":3}"""));
        var str2Json = JsonSerializer.Serialize(str2);
        var dto2 = await _functionCallExecutor.ExecuteAsync(nameof(_demoAppService.GetDemoDtoAsync), BinaryData.FromString("""{"name":"bike","value":99}"""));
        var dto2Json = JsonSerializer.Serialize(dto2);

        //Assert
        str.ShouldBe("hellohellohello");
        dtoJson.ShouldBe("""{"Name":"bike","Value":99}""");
        str2Json.ShouldBe("""{"ToolCallId":"GetStringAsync","Content":[{"Kind":0,"Text":"\u0022hellohellohello\u0022","ImageUri":null,"ImageBytes":null,"ImageBytesMediaType":null,"InputAudioBytes":null,"InputAudioFormat":null,"FileId":null,"FileBytes":null,"FileBytesMediaType":null,"Filename":null,"ImageDetailLevel":null,"Refusal":null}]}""");
        dto2Json.ShouldBe("""{"ToolCallId":"GetDemoDtoAsync","Content":[{"Kind":0,"Text":"get a name:bike and value:99","ImageUri":null,"ImageBytes":null,"ImageBytesMediaType":null,"InputAudioBytes":null,"InputAudioFormat":null,"FileId":null,"FileBytes":null,"FileBytesMediaType":null,"Filename":null,"ImageDetailLevel":null,"Refusal":null}]}""");
    }
}