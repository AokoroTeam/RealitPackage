using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Realit.Core.Managers;
using NaughtyAttributes;
using UnityEngine.InputSystem;
using LTX.Settings;

namespace Realit.Core.Features
{
    public abstract class FeatureData<T> : FeatureDataAsset where T : Feature
    {
        public override Feature GenerateFeature() => GenerateFeatureFromData();

        public abstract T GenerateFeatureFromData();
    }

    public abstract class FeatureDataAsset : ScriptableObject
    {
        [BoxGroup("Global"), SerializeField]
        private string featureName;
        [BoxGroup("Global"), Range(0, 15), Tooltip("15 = max priority")]
        public int loadingPriority = 15;
        [BoxGroup("Global")]
        public bool canLog = true;
        [BoxGroup("Global")]
        public bool isConcurrantFeature = false;
        [BoxGroup("Settings"), SerializeField]
        public bool hasSettings;
        [BoxGroup("Settings"), ShowIf(nameof(hasSettings))]
        public SettingsSection[] settings;

        [BoxGroup("Inputs")]
        public bool hasInputOverride;
        [BoxGroup("Inputs")]
        [ShowIf(nameof(hasInputOverride))]
        public InputAction inputOverride;

        //[HorizontalLine]
        [BoxGroup("GameObjects")]
        public GameObject playerObject;
        [BoxGroup("GameObjects")]
        public GameObject[] sceneObjects;

        [BoxGroup("UI"), ShowAssetPreview]
        public Sprite defaultIcon;
        [BoxGroup("UI"), Range(0, 15)]
        public int uiPriority = 0;
        
        public string FeatureName => featureName;

        public abstract Feature GenerateFeature();

        public virtual bool CanGenerateFeature() => true;

        public virtual SettingsSection[] GetSettings() => settings;

        public override bool Equals(object other)
        {
            return base.Equals(other);
        }

        public override int GetHashCode()
        {
            return featureName.GetHashCode();
        }
        public override string ToString()
        {
            return JsonUtility.ToJson(this, false);
        }
    }
}
