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
    public class ChoiceSettingUI : BaseSettingUI<int>
    {
        protected override SettingType Type => SettingType.Choice;

        [SerializeField]
        private TextMeshProUGUI label;

        
        [SerializeField]
        private CustomDropdown dropdown;

        protected override bool IsDirty() => Setting.Value != CurrentIndex;

        public int CurrentIndex => dropdown.selectedItemIndex;

        public override int GetValueFromUI() => CurrentIndex;

        public override void SetUIFromValue(ISetting<int> setting)
        {
            ChoiceSetting choiceSetting = (ChoiceSetting)setting;
            label.text = choiceSetting.label;

            //Create new ones
            string[] choices = choiceSetting.Choices;

            dropdown.items = new List<CustomDropdown.Item>();

            for (int i = 0; i < choices.Length; i++)
                dropdown.CreateNewItem(choices[i], false);

            dropdown.SetupDropdown();
            dropdown.SetDropdownIndex(choiceSetting.Value);
        }
    }
}
