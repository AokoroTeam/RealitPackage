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
    public class IntSettingUI : BaseSettingUI<int>
    {
        protected override SettingType Type => SettingType.Integer;

        [HorizontalLine]
        [SerializeField]
        private SliderManager sliderManager;
        [SerializeField]
        private TextMeshProUGUI label;
        [Space]
        [SerializeField, ReadOnly]
        private int firstValue;

        protected override bool IsDirty() => firstValue != sliderManager.mainSlider.value;

        public override int GetValueFromUI() => (int)sliderManager.mainSlider.value;
        public override void SetUIFromValue(ISetting<int> setting)
        {
            IntegerSetting intSetting = (IntegerSetting)setting;

            sliderManager.minValue = intSetting.MinMax.x;
            sliderManager.maxValue = intSetting.MinMax.y;

            sliderManager.mainSlider.value = intSetting.Value;
            firstValue = intSetting.Value;

            label.text = setting.Label;

            sliderManager.UpdateUI();
        }
    }
}
