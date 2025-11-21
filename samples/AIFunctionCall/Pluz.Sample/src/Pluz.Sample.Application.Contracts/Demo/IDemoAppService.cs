using System.Threading.Tasks;
using Pluz.Sample.Demo.Dto;
using Volo.Abp.Application.Services;

namespace Pluz.Sample.Demo;

public interface IDemoAppService : IApplicationService
{
    Task DoSthWithoutArgsAsync();

    Task DoSthWithArgsAsync(string arg);

    Task<string> GetStringAsync(string a, int times);

    Task<DemoDto> GetDemoDtoAsync(string name, int value);

    Task WithMoreComplexInputType(ComplexInputDto input);
}