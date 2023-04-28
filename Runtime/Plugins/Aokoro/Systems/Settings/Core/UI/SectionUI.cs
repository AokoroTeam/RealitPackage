using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Aokoro.Settings.UI.Internal;
using TMPro;
using System;
using NaughtyAttributes;

namespace Aokoro.Settings.UI
{
    namespace Internal
    {

        public abstract class SectionUI<UI> : MonoBehaviour, ISettingUIElement<UI> where UI : SettingsUIBuilder
        {
            UI ISettingUIElement<UI>.UIBuilder { get => UIBuilder; set => UIBuilder = value; }

            [SerializeField, ReadOnly]
            public UI UIBuilder;
            public abstract void Populate(SettingsSection section);

            public abstract void ApplyDirtySettings();
        }
    }
    /// <summary>
    /// Component in charge of populating an interface related to given settings sections.
    /// </summary>
    /// <typeparam name="S">Type of components managing Settings UI. Must be a child of BaseSettingUI</typeparam>
    /// <typeparam name="UI">Type of the UI Builder</typeparam>
    public abstract class SectionUI<S, UI> : SectionUI<UI>  where S : BaseSettingUI where UI : SettingsUIBuilder
    {
        [SerializeField]
        protected Transform settingParent;
        
        protected S[] settingsUI;

        public override void Populate(SettingsSection section)
        {
            Clear();
            var settings = section.GetSettings();
            int length = settings.Length;

            settingsUI = new S[length];

            for (int i = 0; i < length; i++)
            {
                ISetting setting = settings[i];
                S settingUI = CreateSettingUI(setting);
                settingsUI[i] = settingUI;

                if (settingUI != null)
                {
                    settingUI.Builder = UIBuilder;
                    settingUI.SettingPointer = new SettingPointer(setting.InternalName, UIBuilder.SettingsHandler);
                    settingUI.SyncUIWithSetting(setting);
                }
            }
        }

        protected virtual S CreateSettingUI(ISetting setting)
        {
            var prefabs = UIBuilder.library.settingsUI;

            for (int i = 0; i < prefabs.Length; i++)
            {
                if (prefabs[i].type == setting.Type)
                {
                    var go = Instantiate(prefabs[i].prefab, settingParent);
                    if (!go.TryGetComponent(out S bsu))
                    {
                        Debug.LogWarning($"[Settings] No BaseSettingUI found on prefab for type {setting.Type}");
                        Destroy(go);
                        return null;
                    }

                    return bsu;
                }
            }

            Debug.LogWarning($"[Settings] No prefab found for type {setting.Type} in library");
            return null;
        }
        public override void ApplyDirtySettings()
        {
            if (settingsUI != null)
            {
                for (int i = 0; i < settingsUI.Length; i++)
                {
                    if (settingsUI[i] != null)
                        settingsUI[i].ApplyIfDirty();
                }
            }
        }
        protected virtual void Clear()
        {
            if (settingsUI != null)
            {
                for (int i = 0; i < settingsUI.Length; i++)
                {
                    if (settingsUI[i] != null)
                        Destroy(settingsUI[i].gameObject);
                }

                settingsUI = null;
            }
        }
    }
}
