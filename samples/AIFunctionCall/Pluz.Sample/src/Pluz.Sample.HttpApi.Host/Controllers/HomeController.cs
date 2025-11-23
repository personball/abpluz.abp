using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;
using VZero.AIFunctionCall;

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

    public ActionResult Tools()
    {
        var text = string.Join("<br/>", _functionCallExecutor.Tools.Select(t => $"[{t.FunctionName}]:{t.FunctionDescription}"));

        return Content(text);
    }

    public async Task<ActionResult> Test(string functionName)
    {
        var message = await _functionCallExecutor.ExecuteAsync(functionName, BinaryData.FromString("""{"a":"hello","times":3}"""));
        return Content(JsonSerializer.Serialize(message));
    }

}
