using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LTX;

namespace LTX.Settings
{
    public class MainSettingsManager : Singleton<MainSettingsManager>
    {
        public static bool IsValid => HasInstance
            && Instance._settingsHandler != null
            && Instance._settingProvider != null
            && Instance._settingsCatalog != null;

        public static SettingsHandler SettingsHandler
        {
            get
            {
                if(Instance._settingsHandler == null && 
                    Instance._settingsCatalog != null && Instance._settingProvider != null)
                    Instance.CreateSettingHandler();

                return Instance._settingsHandler;
            }
        }

        private SettingsHandler _settingsHandler;

        public static SettingsCatalog SettingsCatalog
        {
            get
            {
                return Instance._settingsCatalog;
            }
            set
            {
                Instance._settingsCatalog = value;
                SettingsHandler?.SetCategories(Instance._settingsCatalog);
            }
        }
        [SerializeField, BoxGroup("Data")]
        public SettingsCatalog _settingsCatalog;

        public static ISettingProvider SettingProvider
        {
            get
            {
                return Instance._settingProvider;
            }
            set
            {
                Instance._settingProvider = value;
                SettingsHandler?.SetSettingProvider(Instance._settingProvider);
            }
        }

        [SerializeReference, BoxGroup("Data")]
        private ISettingProvider _settingProvider;
        
        [SerializeField, BoxGroup("Settings")]
        private bool InitializeOnAwake;

        protected override void Awake()
        {
            base.Awake();

            CreateSettingHandler();
        }


        public static bool TryGetSettingValue<T>(string internalName, out T value)
        {
            if (IsValid && SettingsHandler.TryGetSettingValue(internalName, out value))
                return true;

            value = default;
            return false;
        }
        public static bool TrySetSettingValue<T>(string internalName, T value)
        {
            if (IsValid && SettingsHandler.TrySetSettingValue(internalName, value))
                return true;

            return false;
        }

        private void CreateSettingHandler()
        {
            _settingsHandler = new SettingsHandler(_settingProvider, Instance._settingsCatalog == null? new SettingsCategory[0] : Instance._settingsCatalog);
        }


        protected override void OnExistingInstanceFound(MainSettingsManager existingInstance)
        {
            Destroy(gameObject);
        }

    }
}
