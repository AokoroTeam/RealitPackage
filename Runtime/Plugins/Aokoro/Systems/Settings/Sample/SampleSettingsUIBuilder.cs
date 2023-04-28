using Aokoro.Settings.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aokoro.Settings.Samples.UI
{
    [AddComponentMenu("Aokoro/Settings/Builder")]

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
