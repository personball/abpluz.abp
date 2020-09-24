using Volo.Abp.Settings;

namespace Abpluz.Samples.LocalizableContentsSample.Settings
{
    public class LocalizableContentsSampleSettingDefinitionProvider : SettingDefinitionProvider
    {
        public override void Define(ISettingDefinitionContext context)
        {
            //Define your own settings here. Example:
            //context.Add(new SettingDefinition(LocalizableContentsSampleSettings.MySetting1));
        }
    }
}
