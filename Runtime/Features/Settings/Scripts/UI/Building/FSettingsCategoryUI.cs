using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LTX.Settings.UI;
using LTX.Settings;

namespace Realit.Core.Features.Settings.UI
{
    public class FSettingsCategoryUI : CategoryUI<FSettingsSectionUI, FSettingsUIBuilder>
    {
        SettingWindowActions swa;

        private void Awake()
        {
            swa = GetComponentInParent<SettingWindowActions>();
        }

        protected override bool CanCreateSectionUI(SettingsSection settingsSection)
        {
            return settingsSection.Count != 0;
        }
        public void ResetSections()
        {
            for (int i = 0; i < SectionsUI.Length; i++)
            {
                var sectionUI = SectionsUI[i];
                sectionUI.ResetSettings();
            }
        }

        public void CloseWindow()
        {
            swa.CloseWindow();
        }
    }
}
