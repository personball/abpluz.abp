using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Abpluz.Samples.LocalizableContentsSample.Localization;
using Abpluz.Samples.LocalizableContentsSample.MultiTenancy;
using Volo.Abp.TenantManagement.Web.Navigation;
using Volo.Abp.UI.Navigation;

namespace Abpluz.Samples.LocalizableContentsSample.Web.Menus
{
    public class LocalizableContentsSampleMenuContributor : IMenuContributor
    {
        public async Task ConfigureMenuAsync(MenuConfigurationContext context)
        {
            if (context.Menu.Name == StandardMenus.Main)
            {
                await ConfigureMainMenuAsync(context);
            }
        }

        private async Task ConfigureMainMenuAsync(MenuConfigurationContext context)
        {
            if (!MultiTenancyConsts.IsEnabled)
            {
                var administration = context.Menu.GetAdministration();
                administration.TryRemoveMenuItem(TenantManagementMenuNames.GroupName);
            }

            var l = context.GetLocalizer<LocalizableContentsSampleResource>();

            context.Menu.Items.Insert(0, new ApplicationMenuItem(LocalizableContentsSampleMenus.Home, l["Menu:Home"], "~/"));
        }
    }
}
