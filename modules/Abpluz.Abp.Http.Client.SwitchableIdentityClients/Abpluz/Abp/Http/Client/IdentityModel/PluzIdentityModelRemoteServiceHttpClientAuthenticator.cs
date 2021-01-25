using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Http.Client;
using Volo.Abp.Http.Client.Authentication;
using Volo.Abp.IdentityModel;
using Volo.Abp.Users;

namespace Abpluz.Abp.Http.Client.IdentityModel
{
    [Dependency(ReplaceServices = true)]
    public class PluzIdentityModelRemoteServiceHttpClientAuthenticator : IRemoteServiceHttpClientAuthenticator, ITransientDependency
    {
        protected IIdentityModelAuthenticationService IdentityModelAuthenticationService { get; }
        private readonly ICurrentUser _currentUser;

        public PluzIdentityModelRemoteServiceHttpClientAuthenticator(
            IIdentityModelAuthenticationService identityModelAuthenticationService,
            ICurrentUser currentUser)
        {
            IdentityModelAuthenticationService = identityModelAuthenticationService;
            _currentUser = currentUser;
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

            // HACK set __current-user
            if (_currentUser != null && _currentUser.Id.HasValue)
            {
                context.Request.Headers.Add("__current-user", _currentUser.Id.ToString());
            }
        }
    }
}
