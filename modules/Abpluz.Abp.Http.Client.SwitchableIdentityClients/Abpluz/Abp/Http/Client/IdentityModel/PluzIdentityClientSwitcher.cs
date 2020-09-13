using System;
using System.Threading;
using Volo.Abp;

namespace Abpluz.Abp.Http.Client.IdentityModel
{
    public class PluzIdentityClientSwitcher
    {
        public static readonly AsyncLocal<string> Current = new AsyncLocal<string>();

        public static IDisposable Use(string identityClientName)
        {
            var parentScope = Current.Value;

            Current.Value = identityClientName;

            return new DisposeAction(() =>
            {
                Current.Value = parentScope;
            });
        }
    }
}