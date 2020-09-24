using Abpluz.Samples.SwitchableIdentityClientsSample.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Abpluz.Samples.SwitchableIdentityClientsSample.Permissions
{
    public class SwitchableIdentityClientsSamplePermissionDefinitionProvider : PermissionDefinitionProvider
    {
        public override void Define(IPermissionDefinitionContext context)
        {
            var myGroup = context.AddGroup(SwitchableIdentityClientsSamplePermissions.GroupName);

            //Define your own permissions here. Example:
            //myGroup.AddPermission(SwitchableIdentityClientsSamplePermissions.MyPermission1, L("Permission:MyPermission1"));
        }

        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<SwitchableIdentityClientsSampleResource>(name);
        }
    }
}
