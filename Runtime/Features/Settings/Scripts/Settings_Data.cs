using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Realit.Core.Features;
using Realit.Core.Managers;
using NaughtyAttributes;
using Aokoro.Settings;

namespace Realit.Core.Features.Settings
{
    [CreateAssetMenu(fileName = "Settings_Data", menuName = "Aokoro/Realit/Features/Settings/Data")]
    public class Settings_Data : FeatureData<Settings>
    {
        [BoxGroup("UI")]
        public GameObject panel;
        [BoxGroup("UI")]
        public Sprite featuresTabIcon;

        [EnumFlags, SerializeField, BoxGroup("Global")]
        RuntimePlatform supportedPlateforms;

        public override Settings GenerateFeatureFromData()
        {
            return new Settings(this);
        }

        public override bool CanGenerateFeature() => supportedPlateforms.HasFlag(Application.platform);
    }
}