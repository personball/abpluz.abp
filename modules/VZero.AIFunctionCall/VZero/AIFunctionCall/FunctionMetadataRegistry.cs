using System.Collections.Concurrent;
using System.Reflection;

namespace VZero.AIFunctionCall;

internal static class FunctionMetadataRegistry
{
    internal static ConcurrentDictionary<string, FunctionMetadata> Functions = new();

    internal static ConcurrentDictionary<string, (Type, MethodInfo)> ResultConvertMethods = new();

    internal static void MatchResultConvertMethodsToFunctions()
    {
        foreach (var converter in ResultConvertMethods)
        {
            if (!Functions.ContainsKey(converter.Key))
            {
                continue;
            }

            var function = Functions[converter.Key];
            if (function.ResultConvertMethodInfo != null)
            {
                // 同类型的 convert 方法优先级更高
                continue;
            }

            function.ResultConvertServiceType = converter.Value.Item1;
            function.ResultConvertMethodInfo = converter.Value.Item2;
        }
    }
}