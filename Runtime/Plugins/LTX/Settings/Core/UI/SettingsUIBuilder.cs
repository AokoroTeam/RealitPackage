using LTX.Settings.UI.Internal;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LTX.Settings.UI
{
    namespace Internal
    {
        public abstract class SettingsUIBuilder : MonoBehaviour
        {
            [BoxGroup("Data")]
            public SettingsUILibrary library;
            [BoxGroup("Data")]
            private SettingsHandler _settingsHandler;

            private bool needBuild = false;

            public SettingsHandler SettingsHandler
            {
                get => _settingsHandler;
                private set
                {
                    if(_settingsHandler != value)
                    {
                        if (_settingsHandler != null)
                            _settingsHandler.OnCategoryContentChanges -= BuildIfEnabled;

                        _settingsHandler = value;

                        if(_settingsHandler != null)
                            _settingsHandler.OnCategoryContentChanges += BuildIfEnabled;
                        
                        BuildIfEnabled();
                    }
                }
            }

            protected virtual void Start()
            {
                SettingsHandler = GetOrCreateSettingsHandler();
            }

            protected virtual void OnEnable()
            {
                if(needBuild)
                    BuildUI();

                needBuild = false;
            }

            protected abstract SettingsHandler GetOrCreateSettingsHandler();

            private void  BuildIfEnabled()
            {
                if (gameObject.activeInHierarchy)
                    BuildUI();
                else
                    needBuild = true;
            }

            protected virtual void OnDestroy()
            {
                SettingsHandler = null;
            }

            public abstract void ClearUI();
            public abstract void BuildUI();
        }
    }
    /// <summary>
    /// Component in charge of creating an interface related to a given catalog or a category list.
    /// </summary>
    /// <typeparam name="C">Type of components managing categories UI. Must be a child of CategoryUI</typeparam>
    /// <typeparam name="UI">Type of the UI Builder. Most of the time, you should put the inheriting class itself</typeparam>
    public abstract class SettingsUIBuilder<C, UI> : SettingsUIBuilder where C : CategoryUI<UI> where UI : SettingsUIBuilder
    {
        [SerializeField]
        private Transform categoriesParent;


        private Dictionary<string, C> categoryUIManagers = new();


        /// <summary>
        /// Create all UI
        /// </summary>
        /// <param name="categories">Setting data Data</param>
        public override void BuildUI()
        {
            if (categoryUIManagers.Count > 0)
                ClearUI();

            List<SettingsCategory> categories = SettingsHandler.categories;
            foreach (var category in categories)
            {
                if(CanCreateCategoryUI(category))
                    SetupCategory(category);
            }

        }
        /// <summary>
        /// Destroy all created objects and remove all data
        /// </summary>
        public override void ClearUI()
        {
            foreach (var kvp in categoryUIManagers)
                Destroy(kvp.Value);

            categoryUIManagers.Clear();
        }

        /// <summary>
        /// Add category or modifiy existing one
        /// </summary>
        /// <param name="category"></param>
        /// <param name="mergeIfExists">if true, adds sections to existing category. If false, remove existing category and replaces it with the new one.</param>
        /// <returns>Created or modified category</returns>
        protected virtual C SetupCategory(SettingsCategory category)
        {
            if (categoryUIManagers.TryGetValue(category.categoryName, out C baseSettingCategoryUI))
                baseSettingCategoryUI.Populate(category);
            else
            {
                baseSettingCategoryUI = CreateCategoryUI(category);
                categoryUIManagers.Add(category.categoryName, baseSettingCategoryUI);
                baseSettingCategoryUI.Populate(category);
            }

            return baseSettingCategoryUI;
        }
        protected virtual bool CanCreateCategoryUI(SettingsCategory category) => true;

        /// <summary>
        /// Create the category manager that will hold all the data and create all sections.
        /// </summary>
        /// <param name="category"></param>
        /// <returns>Created manager</returns>
        protected virtual C CreateCategoryUI(SettingsCategory category)
        {
            var instance = Instantiate(library.categoryPrefab, categoriesParent);
            C baseSettingCategoryUI = instance.GetComponent<C>();

            if (this is UI uiBuilder)
            {
                baseSettingCategoryUI.UIBuilder = uiBuilder;
                return baseSettingCategoryUI;
                
            }
            else
            {
                Destroy(instance);

                Debug.LogError($"[Settings UI] {library.categoryPrefab.name} category script isn't compatible with a builder of type {GetType().Name}");
                return null;
            }
        }

        public virtual void ApplyDirtySettings()
        {
            foreach (var category in categoryUIManagers)
                category.Value.SetDirtyCategory();

        }

        public virtual void WriteDirtySettings() => SettingsHandler?.WriteDirtySettings();

    }
}
