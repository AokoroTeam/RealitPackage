using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

using Aokoro.Settings;
using Aokoro.Settings.UI;
using Aokoro.Settings.Types;

using TMPro;
using Michsky.MUIP;

namespace Realit.Core.Features.Settings.UI
{
    public class FloatSettingUI : BaseSettingUI
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
        protected override void OnDisable()
        {
            base.OnDisable();
        }

        protected override bool IsDirty() => firstValue != sliderManager.mainSlider.value;

        protected override ISetting SyncSettingWithUI()
        {
            FloatSetting setting = (FloatSetting)Setting;

            setting.SetValue(sliderManager.mainSlider.value);

            return setting;
        }

        public override void SyncUIWithSetting(ISetting setting)
        {
            FloatSetting floatSetting = (FloatSetting)setting;

            sliderManager.mainSlider.minValue = floatSetting.MinMax.x;
            sliderManager.mainSlider.maxValue = floatSetting.MinMax.y;

            sliderManager.mainSlider.value = floatSetting.Value;
            firstValue = floatSetting.Value;

            label.text = setting.Label;

            sliderManager.UpdateUI();
        }
    }
}
