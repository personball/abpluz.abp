using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Pluz.Sample.Demos
{
    public interface IDemoAppService:IApplicationService
    {
        Task AccessWithDefaultPasswordAuthAsync();

        Task AccessWithClientAuthAsync();
    }
}
