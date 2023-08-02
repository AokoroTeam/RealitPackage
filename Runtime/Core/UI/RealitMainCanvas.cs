using Aokoro.UI;
using LTX.ChanneledProperties;
using LTX.Settings;
using LTX.Settings.Types;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Application = UnityEngine.Device.Application;

namespace Realit.Core
{
    public class RealitMainCanvas : UIManager
    {
        [BoxGroup("Scaling"), SerializeField]
        private CanvasScaler canvasScaler;
        [BoxGroup("Scaling"), SerializeField]
        private float mobileScaling;
        [BoxGroup("Scaling"), SerializeField]
        private float desktopScaling;
        [BoxGroup("Scaling"), SerializeField]
        public InfluencedProperty<float> UIScaling;
        public bool IsMobileUI { get; private set; }


        private const string SettingName = "UI_Scale";

        protected override void OnValidate()
        {
            base.OnValidate();
            canvasScaler = GetComponent<CanvasScaler>();
            //canvasScaler.fallbackScreenDPI = canvasScaling;
        }

        protected override void Awake()
        {
            base.Awake();
            UIScaling = new InfluencedProperty<float>(1);

            UIScaling.AddChannel(
                this, 
                Application.isMobilePlatform ? mobileScaling : desktopScaling, 
                Influence.Multiply);

            UIScaling.AddOnValueChangeCallback(OnScalingChanges, true);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (Application.isPlaying)
            {
                UIScaling.AddChannel(MainSettingsManager.Instance, 1, Influence.Multiply, 1);

                MainSettingsManager.AddSettingChangeCallback(SettingName, OnSettingChanges);
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (Application.isPlaying)
            {
                MainSettingsManager.RemoveSettingChangeCallback(SettingName, OnSettingChanges);

                if (MainSettingsManager.TryGetSettingValue(SettingName, out float settingMultiplier))
                    UIScaling.RemoveChannel(MainSettingsManager.Instance);
            }
        }

        private void OnScalingChanges(float scaleFactor)
        {
            Debug.Log(scaleFactor);
            canvasScaler.scaleFactor = scaleFactor;
        }
        private void OnSettingChanges(ISetting setting)
        {
            Debug.Log("a");
            if (setting is ISetting<float> fs)
            {
                Debug.Log("aaa");
                UIScaling.Write(MainSettingsManager.Instance, fs.Value);
            }
        }

        protected override void Update()
        {
            base.Update();

            if (Application.isPlaying)
            {
                if (Application.isMobilePlatform != IsMobileUI)
                    UIScaling.Write(this, Application.isMobilePlatform ? mobileScaling : desktopScaling);
            }
            IsMobileUI = Application.isMobilePlatform;
        }



    }
}
