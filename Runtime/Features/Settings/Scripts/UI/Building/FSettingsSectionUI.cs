using LTX.Settings;
using LTX.Settings.UI;
using TMPro;
using UnityEngine;

namespace Realit.Core.Features.Settings.UI
{
    public class FSettingsSectionUI : SectionUI<BaseSettingUI, FSettingsUIBuilder>
    {
        [SerializeField]
        private TextMeshProUGUI label;


        public override void Populate(SettingsSection section)
        {
            label.text = section.Label;

            base.Populate(section);
        }

        internal void ResetSettings()
        {
            for (int i = 0; i < settingsUI.Length; i++)
            {
                if (settingsUI[i] != null)
                    settingsUI[i].ResetSetting();
            }
        }
    }
}
