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
    public class FloatSettingUI : BaseSettingUI<float>
    {
        protected override SettingType Type => SettingType.Float;

        [SerializeField]
        private SliderManager sliderManager;
        [SerializeField]
        private TextMeshProUGUI label;

        [Space]
        [SerializeField, ReadOnly]
        private float firstValue;


        protected override void OnEnable()
        {
            base.OnEnable();
        }
        
        protected override bool IsDirty() => firstValue != sliderManager.mainSlider.value;


        public override void SetUIFromValue(ISetting<float> setting)
        {
            FloatSetting floatSetting = (FloatSetting)setting;
            sliderManager.mainSlider.minValue = floatSetting.MinMax.x;
            sliderManager.mainSlider.maxValue = floatSetting.MinMax.y;

            sliderManager.mainSlider.value = floatSetting.Value;
            firstValue = floatSetting.Value;

            label.text = setting.Label;

            sliderManager.UpdateUI();
        }

        public override float GetValueFromUI() => sliderManager.mainSlider.value;
    }
}
