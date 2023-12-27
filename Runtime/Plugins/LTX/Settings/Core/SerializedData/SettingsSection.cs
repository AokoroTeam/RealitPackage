using LTX.Settings.Types;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LTX.Settings
{
    
    [System.Serializable]
    public struct SettingsSection 
#if UNITY_EDITOR
        : ISerializationCallbackReceiver
#endif
    {
#if UNITY_EDITOR
        private static bool ApplicationIsRunning = false;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnApplicationLoads()
        {
            ApplicationIsRunning = true;
        }
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        private static void OnApplicationStarts()
        {
            ApplicationIsRunning = false;
        }

        public const string SettingsName = nameof(settingsList);
#endif
        public string Label;

        [SerializeReference]
        private List<ISetting> settingsList;

        public int Count => settingsList.Count;
        public ISetting[] GetSettings() => Settings.ToArray();

        internal List<ISetting> Settings
        {
            get
            {
                settingsList ??= new List<ISetting>();
                return settingsList;
            }
            private set
            {
                settingsList = value;
            }
        }

        public void AddFloat() => Settings.Add(new FloatSetting());
        public void AddInteger() => Settings.Add(new FloatSetting());
        public void AddText() => Settings.Add(new TextSetting());
        public void AddBoolean() => Settings.Add(new BooleanSetting());
        public void AddVector3() => Settings.Add(new Vector3Setting());
        public void AddVector2() => Settings.Add(new Vector2Setting());
        public void AddChoice() => Settings.Add(new ChoiceSetting());

#if UNITY_EDITOR
        public void OnBeforeSerialize()
        {
            if (ApplicationIsRunning)
                return;

            foreach (var setting in settingsList)
            {
                setting.Reset();
                //Debug.Log($"Setting {setting.InternalName} has now value {setting.ToString()}");
            }
        }

        public void OnAfterDeserialize()
        {
            if (ApplicationIsRunning)
                return;

            foreach (var setting in settingsList)
                setting.Reset();
        }
#endif
    }
}
