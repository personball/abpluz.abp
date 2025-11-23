namespace VZero.AIFunctionCall;

public class SampleAppService
{
    [AIFunctionCall(nameof(GetStringAsync), "A demo")]
    public async Task<string> GetStringAsync()
    {
        return "123";
    }
}