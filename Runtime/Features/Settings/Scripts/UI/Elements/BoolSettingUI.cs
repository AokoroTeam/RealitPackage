using Michsky.MUIP;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

using LTX.Settings;
using LTX.Settings.UI;
using LTX.Settings.Types;

namespace Realit.Core.Features.Settings.UI
{
    public class BoolSettingUI : BaseSettingUI<bool>
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


        protected override bool IsDirty() => firstValue != switchManager.isOn;


        public override void SetUIFromValue(ISetting<bool> setting)
        {
            switchManager.isOn = setting.Value;
            firstValue = setting.Value;

            label.text = setting.Label;

            switchManager.UpdateUI();
        }

        public override bool GetValueFromUI() => switchManager.isOn;
    }
}
