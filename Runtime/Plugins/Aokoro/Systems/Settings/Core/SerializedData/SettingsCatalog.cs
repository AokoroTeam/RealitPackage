using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aokoro.Settings
{
    [CreateAssetMenu(fileName = "Settings", menuName = "Aokoro/Settings/Catalog")]
    public class SettingsCatalog : ScriptableObject
    {
        public SettingsCategory[] categories;

        public static implicit operator SettingsCategory[] (SettingsCatalog catalog) => catalog.categories;
    }
}
