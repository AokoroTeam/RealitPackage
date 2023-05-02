using Michsky.MUIP;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

using LTX.Settings;
using LTX.Settings.UI;
using LTX.Settings.Types;

namespace Realit.Core.Features.Settings.UI
{
    public class BoolSettingUI : BaseSettingUI
    {
        protected override SettingType Type => SettingType.Integer;

        [HorizontalLine]
        [SerializeField]
        private SwitchManager switchManager;
        [SerializeField]
        private TextMeshProUGUI label;
        [Space]
        [SerializeField, ReadOnly]
        private bool firstValue;


        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

        protected override bool IsDirty() => firstValue != switchManager.isOn;

        protected override ISetting SyncSettingWithUI()
        {
            BooleanSetting setting = (BooleanSetting)Setting;

            setting.SetValue(switchManager.isOn);

            return setting;
        }

        public override void SyncUIWithSetting(ISetting setting)
        {
            BooleanSetting boolSetting = (BooleanSetting)setting;

            switchManager.isOn = boolSetting.Value;
            firstValue = boolSetting.Value;

            label.text = setting.Label;

            switchManager.UpdateUI();
        }
    }
}
