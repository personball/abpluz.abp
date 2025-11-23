# VZero.AIFunctionCall

VZero.AIFunctionCall 提供 dotnet 类实例的成员方法作为 OpenAI 的 ChatTool，实现基于名称和 LLM 提供的参数执行指定 FunctionCall 的能力。

## 功能特性

通过在类的成员方法上标记 `[AIFunctionCall("FunctionName","FunctionDescription")]`，可实现：

- 自动发现所有 FunctionCall，自动构建 ChatTool 注册所需的 JsonSchema；
- 通过 `IFunctionCallExecutor.ExecuteAsync(functionName, functionArguments)` 调用对应类的实例方法；
- 在执行方法前，基于 JsonSchema 对 LLM 传入的参数进行验证；
- 在维持原业务方法返回类型的基础上，提供绑定自定义的返回值类型转换方法

## 基本用法

在原服务类方法是上进行标记

```csharp
namespace Pluz.Sample.Demo;

public class DemoAppService : SampleAppService, IDemoAppService
{
    // 标记该方法为一个 FunctionCall
    [AIFunctionCall(nameof(DoSthWithArgsAsync), "has input but no return")] 
    // 参数的 Description 用的是 System.ComponentModel.DescriptionAttribute
    public async Task DoSthWithArgsAsync([Description("this is an arg")] string arg) 
    {
        Console.WriteLine($"[{nameof(DoSthWithArgsAsync)}] Called with input:{arg}");
    }
...
```

在 IoC 配置环节调用自动注册发现

```csharp
using Microsoft.Extensions.DependencyInjection; // AddAIFunctionCalls 在这个命名空间下
public override void ConfigureServices(ServiceConfigurationContext context)
{
    // Services 是 IServiceCollection
    // 可以在这里指定多个程序集进行扫描发现
    context.Services.AddAIFunctionCalls(Assembly.GetExecutingAssembly()); 
    // 注册 IFunctionCallExecutor 实现
    context.Services.AddTransient<IFunctionCallExecutor, DefaultFunctionCallExecutor>();
}
```

在任意地方（一般是LLM对话逻辑处理过程）通过 IoC 注入`IFunctionCallExecutor`对接动态的方法调用。通过`IFunctionCallExecutor.Tools`拿到所有的 ChatTool 定义。

```csharp

namespace Pluz.Sample.Controllers;

public class HomeController : AbpController
{
    private readonly IFunctionCallExecutor _functionCallExecutor;
    public HomeController(IFunctionCallExecutor functionCallExecutor)
    {
        _functionCallExecutor = functionCallExecutor;
    }

    public ActionResult Index()
    {
        return Redirect("~/swagger");
    }

    //只是一个示例
    public ActionResult Tools()
    {
        var text = string.Join("<br/>", _functionCallExecutor.Tools.Select(t => $"[{t.FunctionName}]:{t.FunctionDescription}"));

        return Content(text);
    }

    //只是一个示例
    public async Task<ActionResult> Test(string functionName)
    {
        var message = await _functionCallExecutor.ExecuteAsync(functionName, BinaryData.FromString("""{"a":"hello","times":3}"""));
        return Content(JsonSerializer.Serialize(message));
    }
}
```

**注意：** 所有涉及的类应该都是可以实例化的，并且注册到 IoC 容器。`AddAIFunctionCalls`只会提取 ChatTool 相关的元数据，并不会自动帮你注册方法所在的 Service 类型到 IServiceCollection 中。

## 参数验证

参数验证体系使用 `JsonSchema.Net.Generation` 实现，故对于复杂的自定义入参类型的验证规则要使用 `JsonSchema.Net` 的规则。

举个例子，有如下方法

```csharp
[AIFunctionCall(nameof(WithMoreComplexInputType), " just a test for complex input")]
public async Task WithMoreComplexInputType(ComplexInputDto input)
{

    Console.WriteLine($"[{nameof(WithMoreComplexInputType)}] Called");
}
```

`ComplexInputDto` 定义如下

```csharp
using System.Text.Json.Serialization;

namespace Pluz.Sample.Demo.Dto;

public class ComplexInputDto
{
    [Json.Schema.Generation.Required]
    [Json.Schema.Generation.Description("It's a nullable required property")]
    public string? ANullableButRquiredProperty { get; set; }

    [Json.Schema.Generation.MaxLength(10)]
    [Json.Schema.Generation.Description("Its max length is 10.")]
    public string MaxLength10 { get; set; } = string.Empty;

    [Json.Schema.Generation.MinLength(5)]
    [Json.Schema.Generation.Description("Its min length is 5.")]
    public string MinLength5 { get; set; } = string.Empty;


    [Json.Schema.Generation.Description("Something enum")]
    public CategoryEnum Type { get; set; }

    [Json.Schema.Generation.MinItems(2)]
    [Json.Schema.Generation.MaxItems(5)]
    [Json.Schema.Generation.Description("bla bla bla")]
    public CategoryEnum[] CategoryEnums { get; set; } = [];

}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CategoryEnum
{
    Room,
    Bike,
    Plane
}
```

## 自定义返回值类型转换

由于 `IFunctionCallExecutor.ExecuteAsync` 必须返回一个 `ToolChatMessage`，这里提供了一个同样基于反射机制的自定义返回值类型转换的机制。

这样就可以保持原方法在其他场景的适用性，而无需为了对接 LLM 修改返回类型，或者专门写一个 FunctionCallHandler。

情况一，和 FunctionCall 方法在同一个类上，用 `AIFunctionCall` 的第三个参数。

```csharp
public class DemoAppService : SampleAppService, IDemoAppService
{
    [AIFunctionCall(nameof(GetDemoDtoAsync), "将输入的name和value封装成一个DemoDto实例", nameof(ConvertDemoDtoToToolChatMessage))]
    public async Task<DemoDto> GetDemoDtoAsync(string name, int value)
    {
        return new DemoDto { Name = name, Value = value };
    }
    // 转换方法的第一个入参是上面FunctionCall的返回值， functionName 会自动赋值
    public ToolChatMessage ConvertDemoDtoToToolChatMessage(DemoDto input, string functionName) 
    {
        return ChatMessage.CreateToolMessage(functionName, $"get a name:{input.Name} and value:{input.Value}");
    }
    ...
}
```

情况二，在另一个类上，用 `[AIFunctionCallResultConvert("FunctionName")]`

```csharp
public class DemoConvertAppService : SampleAppService, IDemoConvertAppService
{
    // GetDemoDtoAsync 是定义在 DemoAppService 上的方法
    // 转换方法的第一个入参是 GetDemoDtoAsync 的返回值，functionName 会自动赋值
    [AIFunctionCallResultConvert("GetDemoDtoAsync")]
    public ToolChatMessage ConvertDemoDtoToToolChatMessage(DemoDto input, string functionName)
    {
        return ChatMessage.CreateToolMessage(functionName, $"get a name:{input.Name} and value:{input.Value}");
    }
}
```

## Usage in Volo.Abp

See [VZero.Abp.AIFunctionCall](../VZero.Abp.AIFunctionCall/)