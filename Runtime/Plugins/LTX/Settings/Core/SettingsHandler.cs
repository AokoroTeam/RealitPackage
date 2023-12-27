using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LTX.Settings
{
    [System.Serializable]
    public class SettingsHandler
    {
        public event Action OnCategoryContentChanges;
        protected struct SettingPath
        {
            public readonly int category;
            public readonly int section;
            public readonly int setting;

            public SettingPath(int category, int section, int setting)
            {
                this.category = category;
                this.section = section;
                this.setting = setting;
            }
        }

        [SerializeField]
        internal List<SettingsCategory> categories;

        protected Dictionary<string, List<SettingPath>> settingsPath;

        protected List<string> dirtySettings;

        [SerializeField]
        protected ISettingProvider settingProvider;

        private Dictionary<string, List<Action<ISetting>>> callbacks;

        public SettingsHandler(ISettingProvider settingProvider, SettingsCategory[] categories)
        {
            settingsPath = new Dictionary<string, List<SettingPath>>();
            dirtySettings = new List<string>();

            this.settingProvider = settingProvider;
            callbacks = new Dictionary<string, List<Action<ISetting>>>();

            SetCategories(categories);
        }

        public SettingsHandler(ISettingProvider settingProvider, SettingsCatalog catalog) : this(settingProvider, catalog.categories)
        {

        }
        #region Category Management
        public void SetCategories(SettingsCategory[] categories)
        {
            this.categories = new List<SettingsCategory>(categories);

            GeneratePaths();
            ReadAllSettings();

            OnCategoryContentChanges?.Invoke();
        }
        
        public void AddCategories(params SettingsCategory[] newCategories) => AddCategories(false, newCategories);
        public void AddCategories(bool merge, params SettingsCategory[] newCategories)
        {
            int length = this.categories.Count;
            int changeCount = 0;
            for (int i = 0; i < length; i++)
            {
                SettingsCategory existingCategory = this.categories[i];
                for (int j = 0; j < newCategories.Length; j++)
                {
                    var category = newCategories[j];
                    if (existingCategory.categoryName == category.categoryName)
                    {
                        changeCount++;
                        this.categories[i] = merge ? SettingsCategory.Merge(existingCategory, category) : category;
                        break;
                    }
                }
            }

            
            //If it doesn't exist then add a slot
            for (int i = 0; i < newCategories.Length; i++)
            {
                int idx = categories.FindIndex(ctx => ctx.categoryName == newCategories[i].categoryName);
                if (idx == -1)
                {
                    changeCount++;
                    categories.Add(newCategories[i]);
                }
            }
            if(changeCount != 0)
            {
                GeneratePaths();
                OnCategoryContentChanges?.Invoke();
            }
        }

        public void RemoveCategory(string categoryName)
        {
            int idx = categories.FindIndex(ctx => ctx.categoryName == categoryName);

            if (idx != -1)
            {
                categories.RemoveAt(idx);

                GeneratePaths();
                OnCategoryContentChanges?.Invoke();
            }
        }

        #endregion
        protected void GeneratePaths()
        {
            //Clears the old one or creates one on startup
            settingsPath = new Dictionary<string, List<SettingPath>>();
            SettingsCategory[] categories = this.categories.ToArray();

            //For GC
            List<SettingPath> paths;

            for (int i = 0; i < categories.Length; i++)
            {
                var c = categories[i];
                for (int j = 0; j < c.Sections.Length; j++)
                {
                    var s = c.Sections[j];
                    List<ISetting> settingsList = s.Settings;

                    foreach (ISetting setting in settingsList)
                    {
                        int settingIndex = settingsList.IndexOf(setting);
                        if (settingsPath.TryGetValue(setting.InternalName, out paths))
                            paths.Add(new SettingPath(i, j, settingIndex));
                        else
                            settingsPath.Add(setting.InternalName, new() { new(i, j, settingIndex) });
                    }
                }
            }
        }


        public bool TryGetSetting<T>(string internalName, out T setting) where T : ISetting
        {
            var isetting = GetSetting(internalName);
            if(isetting is T tSetting)
            {
                setting = tSetting;
                return true;
            }

            setting = default;
            return false;
        }

        public bool TrySetSetting<T>(string internalName, T setting) where T : ISetting
        {
            if (setting == null)
                return false;

            if (settingsPath.TryGetValue(setting.InternalName, out List<SettingPath> paths))
            {
                foreach (SettingPath p in paths)
                    categories[p.category].Sections[p.section].Settings[p.setting] = setting;

                dirtySettings.Add(setting.InternalName);

                if(callbacks.TryGetValue(internalName, out var actions))
                {
                    foreach(var action in actions)
                        action?.Invoke(setting);
                }

                return true;
            }

            return false;
        }

        protected ISetting GetSetting(SettingPath p) => categories[p.category].Sections[p.section].Settings[p.setting];
        public ISetting<T> GetSetting<T>(string internalName) => GetSetting(internalName) as ISetting<T>;
        public ISetting GetSetting(string internalName)
        {
            if(settingsPath.TryGetValue(internalName, out List<SettingPath> paths))
            {
                SettingPath p = paths[0];
                return GetSetting(p);
            }
            else
            {
                return null;
            }
        }

        
        #region GET SET values
        public bool TryGetSettingValue<T>(string internalName, out T value)
        {
            if(TryGetSetting(internalName, out ISetting<T> s))
            {
                value = s.Value;
                return true;
            }

            value = default;
            return false;
        }

        public bool TrySetSettingValue<T>(string internalName, T value)
        {
            if(TryGetSetting(internalName, out ISetting<T> s))
            {
                s.SetValue(value);
                return TrySetSetting(internalName, s);
            }

            return false;
        }

        public bool HasSetting(string internalName) => settingsPath.ContainsKey(internalName);
        #endregion
        #region Writting and reading

        private void WriteSetting(ISetting s)
        {
            if (!settingProvider.TryWriteSetting(ref s))
                Debug.LogError($"[Settings] Couldn't write setting {s.InternalName}");
        }


        internal virtual void WriteDirtySettings()
        {
            foreach (var settingInternalName in dirtySettings)
                WriteSetting(GetSetting(settingInternalName));

            dirtySettings.Clear();
        }

        internal virtual void WriteAllSettings()
        {
            foreach (var kvp in settingsPath)
            {
                var s = GetSetting(kvp.Key);
                WriteSetting(s);
            }
        }


        private void ReadSetting(ISetting setting)
        {
            //If the setting doesn't exist yet then reset it
            if (!settingProvider.TryReadSetting(ref setting))
                setting.Reset();

            TrySetSetting(setting.InternalName, setting);
        }

        public virtual void ReadAllSettings()
        {
            if (settingsPath == null || settingProvider == null)
                return;

            foreach(var kvp in settingsPath)
            {
                string internalName = kvp.Key;
                ReadSetting(GetSetting(internalName));
            }
        }

        #endregion

        public void SetSettingProvider(ISettingProvider settingProvider, bool refresh = true)
        {
            if (settingProvider != this.settingProvider && settingProvider != null)
            {
                this.settingProvider = settingProvider;
                if(refresh)
                    ReadAllSettings();

                foreach(var (internalName, actions) in callbacks)
                {
                    foreach (var action in actions)
                        action?.Invoke(GetSetting(internalName));
                }
            }
        }

        public void ManuallyTriggerContentChange() => OnCategoryContentChanges?.Invoke();

        public void AddSettingChangeCallback(string internalName, Action<ISetting> callback)
        {
            if(!callbacks.TryGetValue(internalName, out List<Action<ISetting>> list))
            {
                list = new List<Action<ISetting>>();
                callbacks.Add(internalName, list);
            }

            if(!list.Contains(callback))
                list.Add(callback);
        }

        public void RemoveSettingChangeCallback(string internalName, Action<ISetting> callback)
        {
            if (!callbacks.TryGetValue(internalName, out List<Action<ISetting>> list))
                return;

            list?.Remove(callback);
        }
    }
}
