using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LTX.Settings
{
    [CreateAssetMenu(fileName = "Settings", menuName = "LTX/Settings/Catalog")]
    public class SettingsCatalog : ScriptableObject
    {
        public SettingsCategory[] categories;

        public static implicit operator SettingsCategory[] (SettingsCatalog catalog) => catalog.categories;
    }
}
