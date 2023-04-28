using Aokoro.Settings.Types;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aokoro.Settings
{
    [System.Serializable]
    public struct SettingsSection
    {
#if UNITY_EDITOR
        public const string SettingsName = nameof(settingsList);
#endif
        public string Label;

        [SerializeReference]
        private List<ISetting> settingsList;

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
    }
}
