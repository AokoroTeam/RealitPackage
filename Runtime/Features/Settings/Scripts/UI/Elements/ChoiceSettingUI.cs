using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

using LTX.Settings;
using LTX.Settings.UI;
using LTX.Settings.Types;

using TMPro;
using Michsky.MUIP;
using UnityEngine.UI;

namespace Realit.Core.Features.Settings.UI
{
    public class ChoiceSettingUI : BaseSettingUI
    {
        protected override SettingType Type => SettingType.Choice;

        [SerializeField]
        private TextMeshProUGUI label;

        [SerializeField]
        private int firstValue;

        [SerializeField]
        private CustomDropdown dropdown;

        protected override bool IsDirty() => firstValue != CurrentIndex;

        public int CurrentIndex { get; private set; }

        public void OnValueChanged(int index) => CurrentIndex = index;
        protected override ISetting SyncSettingWithUI()
        {
            ChoiceSetting setting = (ChoiceSetting)Setting;

            setting.SetValue(CurrentIndex);

            return setting;
        }

        public override void SyncUIWithSetting(ISetting setting)
        {
            ChoiceSetting choiceSetting = (ChoiceSetting)setting;

            firstValue = choiceSetting.Value;
            label.text = choiceSetting.label;
            
            //Create new ones
            string[] choices = choiceSetting.Choices;

            dropdown.items = new List<CustomDropdown.Item>();

            for (int i = 0; i < choices.Length; i++)
                dropdown.CreateNewItem(choices[i], false);
            
            dropdown.SetupDropdown();
            dropdown.SetDropdownIndex(firstValue);
        }
    }
}
