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


        public SettingsHandler(ISettingProvider settingProvider, SettingsCategory[] categories)
        {
            settingsPath = new Dictionary<string, List<SettingPath>>();
            dirtySettings = new List<string>();

            this.settingProvider = settingProvider;
            SetCategories(categories);
        }

        public SettingsHandler(ISettingProvider settingProvider, SettingsCatalog catalog) : this(settingProvider, catalog.categories)
        {

        }

        public void SetCategories(SettingsCategory[] categories)
        {
            this.categories = new List<SettingsCategory>(categories);

            GeneratePaths();
            ReadSettings();

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
                    var category = newCategories[i];
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

        public ISetting GetSettingWithInternalName(string internalName)
        {
            if(settingsPath.TryGetValue(internalName, out List<SettingPath> paths))
            {
                SettingPath p = paths[0];
                return GetSettingWithPath(p);
            }
            else
            {
                return null;
            }
        }

        protected ISetting GetSettingWithPath(SettingPath p) => categories[p.category].Sections[p.section].Settings[p.setting];
        public bool ApplySetting(ISetting setting, bool addToDirtyCollection = true)
        {
            if (setting == null)
                return false;

            if (settingsPath.TryGetValue(setting.InternalName, out List <SettingPath> paths))
            {
                foreach (SettingPath p in paths)
                    categories[p.category].Sections[p.section].Settings[p.setting] = setting;

                if(addToDirtyCollection)
                    dirtySettings.Add(setting.InternalName);

                return true;
            }

            return false;
        }
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
        public bool TryGetSettingValue<T>(string settingInternalName, out T value)
        {
            var setting = GetSettingWithInternalName(settingInternalName);
            if(setting is ISetting<T> s)
            {
                value = s.Value;
                return true;
            }
            
            value = default;
            return false;
        }
        public bool TrySetSettingValue<T>(string settingInternalName, T value)
        {
            var setting = GetSettingWithInternalName(settingInternalName);
            if (setting is ISetting<T> s)
            {
                WriteSetting(settingInternalName, ref s);
                return true;
            }

            return false;
        }

        public void WriteAllDirtySettings() => WriteSettings(true);

        internal virtual void WriteSettings(bool onlyDirty)
        {
            if (onlyDirty)
            {
                foreach (var settingInternalName in dirtySettings)
                {
                    var s = GetSettingWithInternalName(settingInternalName);
                    WriteSetting(settingInternalName, ref s);
                }
                dirtySettings.Clear();
            }
            else
            {
                foreach(var kvp in settingsPath) 
                {
                    var s = GetSettingWithInternalName(kvp.Key);
                    if (!settingProvider.TryWriteSetting(ref s))
                        Debug.LogError($"[Settings] Couldn't write setting {kvp.Key}");
                    else
                    {
                        //In case setting was changed because of writting
                        ApplySetting(s, false);
                    }
                }
            }
        }
        private void WriteSetting<T>(string settingInternalName, ref ISetting<T> s) => WriteSetting(settingInternalName, ref s);
        private void WriteSetting(string settingInternalName, ref ISetting s)
        {
            if (!settingProvider.TryWriteSetting(ref s))
                Debug.LogError($"[Settings] Couldn't write setting {settingInternalName}");
            else
            {
                //In case setting was changed because of writting
                ApplySetting(s, false);
            }
        }

        public virtual void ReadSettings()
        {
            if (settingsPath == null || settingProvider == null)
                return;

            foreach(var kvp in settingsPath)
            {
                string internalName = kvp.Key;

                ISetting s = GetSettingWithInternalName(internalName);

                //If the setting doesn't exist yet then reset it
                if (!settingProvider.TryReadSetting(ref s))
                    s.Reset();

                ApplySetting(s, false);
            }
        }

        public void SetSettingProvider(ISettingProvider settingProvider, bool refresh = true)
        {
            if (settingProvider != this.settingProvider && settingProvider != null)
            {
                this.settingProvider = settingProvider;
                if(refresh)
                    ReadSettings();
            }
        }

        public void ManuallyTriggerContentChange() => OnCategoryContentChanges?.Invoke();
    }
}
