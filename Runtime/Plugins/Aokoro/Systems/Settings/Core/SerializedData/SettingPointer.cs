using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aokoro.Settings
{
    [System.Serializable]
    public struct SettingPointer 
    {
        public string internalName;
        public SettingsHandler handler;

        public ISetting GetSetting() => handler.GetSettingWithInternalName(internalName);

        public SettingPointer(string internalName, SettingsHandler handler)
        {
            this.internalName = internalName;
            this.handler = handler;
            
        }
    }
}
