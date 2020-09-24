using Abpluz.Samples.LocalizableContentsSample.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Abpluz.Samples.LocalizableContentsSample.Permissions
{
    public class LocalizableContentsSamplePermissionDefinitionProvider : PermissionDefinitionProvider
    {
        public override void Define(IPermissionDefinitionContext context)
        {
            var myGroup = context.AddGroup(LocalizableContentsSamplePermissions.GroupName);

            //Define your own permissions here. Example:
            //myGroup.AddPermission(LocalizableContentsSamplePermissions.MyPermission1, L("Permission:MyPermission1"));
        }

        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<LocalizableContentsSampleResource>(name);
        }
    }
}
