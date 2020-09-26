using System;
using System.Threading.Tasks;
using Abpluz.Abp.Http.Client.IdentityModel;
using Pluz.Sample.Demos;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Identity;

namespace Pluz.Sample.HttpApi.Client.ConsoleTestApp
{
    public class ClientDemoService : ITransientDependency
    {
        private readonly IProfileAppService _profileAppService;
        private readonly IDemoAppService _demoAppService;
        public ClientDemoService(
            IProfileAppService profileAppService,
            IDemoAppService demoAppService)
        {
            _profileAppService = profileAppService;
            _demoAppService = demoAppService;
        }

        public async Task RunAsync()
        {
            var output = await _profileAppService.GetAsync();
            Console.WriteLine($"UserName : {output.UserName}");
            Console.WriteLine($"Email    : {output.Email}");
            Console.WriteLine($"Name     : {output.Name}");
            Console.WriteLine($"Surname  : {output.Surname}");

            await _demoAppService.AccessWithDefaultPasswordAuthAsync();

            // await _demoAppService.AccessWithClientAuthAsync();// will throw

            using (PluzIdentityClientSwitcher.Use("client"))
            {
                await _demoAppService.AccessWithClientAuthAsync();// will not throw
            }
        }
    }
}