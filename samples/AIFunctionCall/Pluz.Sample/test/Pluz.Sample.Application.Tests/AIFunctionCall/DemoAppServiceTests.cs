using Json.Schema;
using OpenAI.Chat;
using Pluz.Sample.Demo.Dto;
using Pluz.Sample.Samples;
using Shouldly;
using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using VZero.Abp.AIFunctionCall;
using Xunit;

namespace Pluz.Sample.Demo;

/// <summary>
/// 不能直接在这个项目里实例化测试类，缺了其他底层模块，
/// 需要在Pluz.Sample.EntityFrameworkCore.Tests中继承本类实现一个子类
/// </summary>
/// <typeparam name="TStartupModule"></typeparam>
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

    tools.Count.ShouldBe(5);

    var schema = tools.First(t => t.FunctionName == nameof(_demoAppService.DoSthWithArgsAsync)).FunctionParameters.ToString();
    schema.ShouldBe("""{"type":"object","properties":{"arg":{"type":"string","description":"this is an arg"}},"required":["arg"]}""");

    var complexInputSchema = tools.First(t => t.FunctionName == nameof(_demoAppService.WithMoreComplexInputType)).FunctionParameters.ToString();
    complexInputSchema.ShouldBe("""{"type":"object","properties":{"input":{"type":"object","properties":{"aNullableButRquiredProperty":{"type":["string","null"],"description":"It\u0027s a nullable required property"},"maxLength10":{"type":"string","maxLength":10,"description":"Its max length is 10."},"minLength5":{"type":"string","minLength":5,"description":"Its min length is 5."},"type":{"$ref":"#/$defs/categoryEnum","description":"Something enum"},"categoryEnums":{"minItems":2,"maxItems":5,"description":"bla bla bla","type":"array","items":{"$ref":"#/$defs/categoryEnum"}}},"required":["aNullableButRquiredProperty"],"$defs":{"categoryEnum":{"enum":["Room","Bike","Plane"]}}}},"required":["input"],"$defs":{"categoryEnum":{"enum":["Room","Bike","Plane"]}}}""");
  }

  private string RenderInvalidMessage(string functionName, string text)
  {
    return JsonSerializer.Serialize(ChatMessage.CreateToolMessage(functionName, text));
  }

  [Fact]
  public async Task ComplexInput_Should_Work()
  {
    var input = new ComplexInputDto
    {
      ANullableButRquiredProperty = null,
      CategoryEnums = [],
      MaxLength10 = "",
      MinLength5 = "",
      Type = CategoryEnum.Room,
    };
    var arg = new { input };

    var message = await _functionCallExecutor.ExecuteAsync(nameof(_demoAppService.WithMoreComplexInputType),
        BinaryData.FromString(JsonSerializer.Serialize(arg, JsonSerializerOptions.Web)));
    var messageJson = JsonSerializer.Serialize(message);
    messageJson.ShouldBe(
      RenderInvalidMessage(nameof(_demoAppService.WithMoreComplexInputType),
      "Function Arguments Not Valid: for /properties/input/properties/minLength5 [minLength] Value should be at least 5 characters"));

    // TODO: verify more parameters

  }

  [Fact]
  public async Task Schema_Evaluate_Test01()
  {
    var inputText = """
{
  "input": {
    "aNullableButRquiredProperty": null,
    "maxLength10": "",
    "minLength5": "",
    "type": "Room",
    "categoryEnums": []
  }
}
""";

    var schemaText = """
{
  "type": "object",
  "properties": {
    "input": {
      "type": "object",
      "properties": {
        "aNullableButRquiredProperty": {
          "type": ["string", "null"],
          "description": "It\u0027s a nullable required property"
        },
        "maxLength10": {
          "type": "string",
          "maxLength": 10,
          "description": "Its max length is 10."
        },
        "minLength5": {
          "type": "string",
          "minLength": 5,
          "description": "Its min length is 5."
        },
        "type": {
          "$ref": "#/$defs/categoryEnum",
          "description": "Something enum"
        },
        "categoryEnums": {
          "description": "bla bla bla",
          "type": "array",
          "items": { "$ref": "#/$defs/categoryEnum" }
        }
      },
      "required": ["aNullableButRquiredProperty"],
      "$defs": { "categoryEnum": { "enum": ["Room", "Bike", "Plane"] } }
    }
  },
  "required": ["input"],
  "$defs": { "categoryEnum": { "enum": ["Room", "Bike", "Plane"] } }
}
""";

    var schema = JsonSchema.FromText(schemaText);
    var result = schema.Evaluate(JsonNode.Parse(inputText), new EvaluationOptions { OutputFormat = OutputFormat.Hierarchical });

    var resultJson = JsonSerializer.Serialize(result);

    result.IsValid.ShouldBe(false);
    // result.Errors.ShouldNotBeNull(); // Fail
    // result.HasErrors.ShouldBe(true); // Fail
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