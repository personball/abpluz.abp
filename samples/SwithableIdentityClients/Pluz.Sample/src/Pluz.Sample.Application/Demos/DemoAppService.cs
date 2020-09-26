using System;
using System.Threading.Tasks;

namespace Pluz.Sample.Demos
{
    public class DemoAppService : SampleAppService, IDemoAppService
    {
        public async Task AccessWithClientAuthAsync()
        {
            if (CurrentUser.Id.HasValue)
            {
                throw new Exception("client auth should not has userId!");
            }
        }

        public async Task AccessWithDefaultPasswordAuthAsync()
        {
            if (!CurrentUser.Id.HasValue)
            {
                throw new Exception("password auth should has userId!");
            }
        }
    }
}
