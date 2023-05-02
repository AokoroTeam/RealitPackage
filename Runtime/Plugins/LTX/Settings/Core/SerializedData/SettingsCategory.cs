using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LTX.Settings
{
    [System.Serializable]
    public struct SettingsCategory
    {
#if UNITY_EDITOR
        public const string sectionPropertyName = nameof(sections);
        public const string categoryLabelPropertyName= nameof(categoryName);
        public const string iconPropertyName = nameof(icon);
#endif
        public string categoryName;
        public Sprite icon;

        [SerializeField]
        private SettingsSection[] sections;

        public SettingsSection[] Sections
        {
            get => sections;
            private set => sections = value;
        }


        public SettingsCategory(string categoryName, Sprite icon, SettingsSection[] sections)
        {
            this.categoryName = categoryName;
            this.icon = icon;
            this.sections = sections;
        }

        public static SettingsCategory[] Merge(SettingsCategory[] bases, SettingsCategory[] overrides)
        {
            List<SettingsCategory> result = new List<SettingsCategory>(bases);

            for (int i = 0; i < overrides.Length; i++)
            {
                bool exists = false;
                var overrideCategory = overrides[i];

                for (int j = 0; j < bases.Length; j++)
                {
                    var baseCategory = bases[j];
                    if(baseCategory.categoryName == overrideCategory.categoryName)
                    {
                        var merge = SettingsCategory.Merge(baseCategory, overrideCategory);

                        int index = result.FindIndex(ctx => ctx.categoryName == overrideCategory.categoryName);
                        if(index != -1)
                        {
                            exists = true;
                            result[index] = merge;
                        }
                        else
                            overrideCategory = merge;

                        break;
                    }
                }

                if(!exists)
                    result.Add(overrideCategory);
            }

            return bases;
        }
        public static SettingsCategory Merge(SettingsCategory @base, SettingsCategory @override)
        {
            List<SettingsSection> baseSections = new List<SettingsSection>(@base.sections);

            SettingsSection[] overrideSections = @override.sections;
            int lenght = overrideSections.Length;

            for (int i = 0; i < lenght; i++)
            {
                int idx = baseSections.FindIndex(ctx => ctx.Label == overrideSections[i].Label);
                //Commune section
                if (idx != -1)
                {
                    var baseSectionSettings = baseSections[idx].Settings;
                    var overrideSectionSettings = overrideSections[i].Settings;

                    foreach (var setting in overrideSectionSettings)
                    {
                        var idx2 = baseSectionSettings.FindIndex(ctx => ctx.Label == setting.Label);

                        if (idx2 == -1)
                            baseSectionSettings.Add(setting);
                        else
                            baseSectionSettings[idx2] = setting;
                    }

                    //baseSections[idx].Settings = baseSectionSettings;
                }
            }

            return new SettingsCategory(@base.categoryName, @base.icon, baseSections.ToArray());
        }
    }
}
