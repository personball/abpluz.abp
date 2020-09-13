using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Http.Client;
using Volo.Abp.Http.Client.Authentication;
using Volo.Abp.IdentityModel;

namespace Abpluz.Abp.Http.Client.IdentityModel
{
    [Dependency(ReplaceServices = true)]
    public class PluzIdentityModelRemoteServiceHttpClientAuthenticator : IRemoteServiceHttpClientAuthenticator, ITransientDependency
    {
        protected IIdentityModelAuthenticationService IdentityModelAuthenticationService { get; }

        public PluzIdentityModelRemoteServiceHttpClientAuthenticator(
            IIdentityModelAuthenticationService identityModelAuthenticationService)
        {
            IdentityModelAuthenticationService = identityModelAuthenticationService;
        }

        public virtual async Task Authenticate(RemoteServiceHttpClientAuthenticateContext context)
        {
            var clientName = context.RemoteService.GetIdentityClient();

            if (!PluzIdentityClientSwitcher.Current.Value.IsNullOrWhiteSpace())
            {
                clientName = PluzIdentityClientSwitcher.Current.Value;
            }

            await IdentityModelAuthenticationService.TryAuthenticateAsync(
                context.Client,
                clientName
            );
        }
    }
}
