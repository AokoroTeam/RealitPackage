using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

using LTX.Settings;
using LTX.Settings.UI;
using LTX.Settings.Types;

using TMPro;
using Michsky.MUIP;

namespace Realit.Core.Features.Settings.UI
{
    public class TextSettingUI : BaseSettingUI
    {
        protected override SettingType Type => SettingType.Text;

        [SerializeField]
        private CustomInputField inputField;
        [SerializeField]
        private TextMeshProUGUI label;

        [Space]
        [SerializeField, ReadOnly]
        private string firstValue;


        protected override void OnEnable()
        {
            base.OnEnable();
        }
        protected override void OnDisable()
        {
            base.OnDisable();
        }

        protected override bool IsDirty() => firstValue != inputField.inputText.text;

        protected override ISetting SyncSettingWithUI()
        {
            TextSetting setting = (TextSetting)Setting;

            setting.SetValue(inputField.inputText.text);

            return setting;
        }

        public override void SyncUIWithSetting(ISetting setting)
        {
            TextSetting textSetting = (TextSetting)setting;

            inputField.inputText.SetTextWithoutNotify(textSetting.Value);
            firstValue = textSetting.Value;

            label.text = setting.Label;

            inputField.UpdateStateInstant();
        }
    }
}
