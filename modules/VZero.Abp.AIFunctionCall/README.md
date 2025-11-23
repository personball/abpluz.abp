# VZero.Abp.AIFunctionCall

仅提供了一个 Volo.Abp 的模块定义，并在模块初始化过程中自动注册了 `IFunctionCallExecutor` 的实现。  

注意 `IServiceCollection.AddAIFunctionCalls(assembly1,assembly2)` 必须手动调用，因为要扫描的程序集必须手动指定。

其他用法见 [VZero.AIFunctionCall](../VZero.AIFunctionCall/)。