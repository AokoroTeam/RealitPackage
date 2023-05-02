using LTX.Settings.UI;
using UnityEngine;

namespace LTX.Settings.Samples.UI
{
    [AddComponentMenu("LTX/Settings/Builder")]

    public class SampleSettingsUIBuilder : SettingsUIBuilder<SampleCategoryUI, SampleSettingsUIBuilder>
    {
        [SerializeField]
        private SettingsCatalog catalog;

        protected override SettingsHandler GetOrCreateSettingsHandler()
        {
            return new SettingsHandler(new SampleSettingsProvider(), catalog.categories);
        }
    }
}
