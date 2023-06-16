using LTX.Settings.UI.Internal;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace LTX.Settings.UI
{
    namespace Internal
    {
        public abstract class CategoryUI<UI> : MonoBehaviour, ISettingUIElement<UI> where UI : SettingsUIBuilder
        {
            UI ISettingUIElement<UI>.UIBuilder { get => UIBuilder; set => UIBuilder = value; }
         
            [SerializeField, ReadOnly]
            public UI UIBuilder;


            public abstract void Populate(SettingsCategory category);

            internal abstract void ApplyDirtySettings();
        }
    }

    /// <summary>
    /// Component in charge of populating an interface related to given settings category.
    /// </summary>
    /// <typeparam name="S">Type of components managing Sections UI. Must be a child of SectionUI</typeparam>
    /// <typeparam name="UI">Type of the UI Builder</typeparam>
    public abstract class CategoryUI<S, UI> : CategoryUI<UI> where S : SectionUI<UI> where UI : SettingsUIBuilder
    {
        [SerializeField]
        Transform sectionsParent;
        
        public S[] SectionsUI { get; private set; }

        public override void Populate(SettingsCategory category)
        {
            SettingsSection[] sections = category.Sections;
            Clear();

            SectionsUI = new S[sections.Length];

            for (int i = 0; i < sections.Length; i++)
            {
                if (CanCreateSectionUI(sections[i]))
                    SetupSection(sections, i);
            }
        }

        protected virtual bool CanCreateSectionUI(SettingsSection settingsSection) => true;

        protected virtual void SetupSection(SettingsSection[] sections, int i)
        {
            SettingsSection section = sections[i];
            S sectionUI = CreateSectionUI(section);
            SectionsUI[i] = sectionUI;

            if (sectionUI != null)
            {
                sectionUI.UIBuilder = UIBuilder;
                sectionUI.Populate(section);
            }
        }

        protected virtual S CreateSectionUI(SettingsSection settingsSection)
        {
            var prefab = UIBuilder.library.sectionPrefab;
            var go = Instantiate(prefab, sectionsParent);

            if(go.TryGetComponent(out S sectionUI))
                return sectionUI;

            Destroy(go);
            return null;
        }
        internal override void ApplyDirtySettings()
        {
            for (int i = 0; i < SectionsUI.Length; i++)
            {
                if (SectionsUI[i] != null)
                    SectionsUI[i].ApplyDirtySettings();
            }
        }
        protected virtual void Clear() 
        {
            if (SectionsUI != null)
            {
                for (int i = 0; i < SectionsUI.Length; i++)
                {
                    if (SectionsUI[i] != null)
                        Destroy(SectionsUI[i].gameObject);
                }

                SectionsUI = null;
            }
        }
    }
}
