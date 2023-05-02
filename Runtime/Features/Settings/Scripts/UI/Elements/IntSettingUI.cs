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
    public class IntSettingUI : BaseSettingUI
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
            IntegerSetting setting = (IntegerSetting)Setting;

            setting.SetValue((int)sliderManager.mainSlider.value);

            return setting;
        }

        public override void SyncUIWithSetting(ISetting setting)
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
