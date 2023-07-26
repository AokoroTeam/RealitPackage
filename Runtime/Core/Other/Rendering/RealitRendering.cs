using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using NaughtyAttributes;
using LTX.Settings;
using System;
using UnityEngine.Rendering.Universal;
using WFA;

namespace Realit.Core
{
    public class RealitRendering : MonoBehaviour
    {
        private const string QualityLevelSetting = "QualityLevel";
        private const string TargetFrameRateSetting = "TargetFramerate";

        [SerializeField]
        WebGLFPSAccelerator accelerator;

        private void Awake()
        {
            MainSettingsManager.SettingsHandler.AddCallback(QualityLevelSetting, OnQualityLevelChanges);
            MainSettingsManager.SettingsHandler.AddCallback(TargetFrameRateSetting, OnTargetFrameRateChanges);

            if(MainSettingsManager.TryGetSettingValue(QualityLevelSetting, out int qualityLevel))
            {
                QualitySettings.SetQualityLevel(qualityLevel);
            }

            if (MainSettingsManager.TryGetSettingValue(TargetFrameRateSetting, out int fpsChoice))
            {
                Application.targetFrameRate = fpsChoice switch
                {
                    0 => 15,
                    1 => 30,
                    2 => 60,
                    _ => -1,
                };
            }
        }

#if UNITY_WEBGL
        private void Update()
        {
            int maxFPS = Application.targetFrameRate == -1 ? 60 : Application.targetFrameRate;
            accelerator._mainData.fpsMax = maxFPS;
        }
#endif

        private void OnDestroy()
        {
            MainSettingsManager.SettingsHandler.RemoveCallback(QualityLevelSetting, OnQualityLevelChanges);
            MainSettingsManager.SettingsHandler.RemoveCallback(TargetFrameRateSetting, OnTargetFrameRateChanges);
        }

        private void OnQualityLevelChanges(ISetting setting)
        {
            if(setting is ChoiceSetting choice)
            {
                QualitySettings.SetQualityLevel(choice.Value);
            }
        }

        private void OnTargetFrameRateChanges(ISetting setting)
        {
            if (setting is ChoiceSetting choice)
            {
                int value = choice.Value;

                Application.targetFrameRate = value switch
                {
                    0 => 15,
                    1 => 30,
                    2 => 60,
                    _ => -1,
                };
            }
        }
    }
}
