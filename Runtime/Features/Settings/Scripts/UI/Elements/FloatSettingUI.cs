using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

using LTX.Settings;
using LTX.Settings.UI;
using LTX.Settings.Types;

using TMPro;
using Michsky.MUIP;
using System.Linq;

namespace Realit.Core.Features.Settings.UI
{
    public class FloatSettingUI : BaseSettingUI<float>
    {
        protected override SettingType Type => SettingType.Float;

        [SerializeField]
        private SliderManager sliderManager;
        [SerializeField]
        private TextMeshProUGUI label;

        [Space]
        [SerializeField, ReadOnly]
        private float currentValue;

        [SerializeField]
        private FloatSetting.LimitedValueRange limitedValueRange;
        private bool hasInfiniteValue = false;

        public void AjustValue(float newValue)
        {
            if (!hasInfiniteValue)
                currentValue = limitedValueRange.GetClosestValue(newValue);
            else
                currentValue = newValue;

            sliderManager.mainSlider.SetValueWithoutNotify(currentValue);
        }
        protected override bool IsDirty() => Setting.Value != currentValue;


        public override void SetUIFromValue(ISetting<float> setting)
        {
            FloatSetting floatSetting = (FloatSetting)setting;
            hasInfiniteValue = floatSetting.hasInfiniteValue;
            limitedValueRange = floatSetting.limitedValues;

            if(hasInfiniteValue)
            {
                sliderManager.mainSlider.minValue = floatSetting.minMax.x;
                sliderManager.mainSlider.maxValue = floatSetting.minMax.y;
            }
            else
            {
                sliderManager.mainSlider.minValue = limitedValueRange.values.Min();
                sliderManager.mainSlider.maxValue = limitedValueRange.values.Max();
            }

            currentValue = floatSetting.Value;
            sliderManager.mainSlider.SetValueWithoutNotify(currentValue);

            label.text = setting.Label;

            sliderManager.UpdateUI();
        }

        public override float GetValueFromUI()
        {
            return currentValue;
        }
    }
}
