using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

using LTX.Settings;
using LTX.Settings.UI;
using LTX.Settings.Types;

using TMPro;
using Michsky.MUIP;
using UnityEngine.TextCore.Text;

namespace Realit.Core.Features.Settings.UI
{
    public class TextSettingUI : BaseSettingUI<string>
    {
        protected override SettingType Type => SettingType.Text;

        [SerializeField]
        private CustomInputField inputField;
        [SerializeField]
        private TextMeshProUGUI label;

        [Space]
        [SerializeField, ReadOnly]
        private string firstValue;



        protected override bool IsDirty() => firstValue != inputField.inputText.text;

        public override string GetValueFromUI() => inputField.inputText.text;

        public override void SetUIFromValue(ISetting<string> setting)
        {
            inputField.inputText.SetTextWithoutNotify(setting.Value);
            firstValue = setting.Value;

            label.text = setting.Label;

            inputField.UpdateStateInstant();
        }
    }
}
