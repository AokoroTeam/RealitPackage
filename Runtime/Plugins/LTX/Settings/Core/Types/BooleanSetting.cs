using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LTX.Settings.Types
{

    [System.Serializable]
    public struct BooleanSetting : ISetting<bool>
    {
        #region Interface
        bool ISetting<bool>.Value => Value;
        bool ISetting<bool>.DefaultValue => DefaultValue;
        string ISetting.Label => label;
        string ISetting.InternalName => internalName;
        string ISetting.Infos => infos;
        #endregion

        [BoxGroup("Infos")]
        public string label;
        [BoxGroup("Infos")]
        public string internalName;
        [BoxGroup("Infos")]
        [ResizableTextArea, AllowNesting]
        public string infos;

        [BoxGroup("Value")]
        public bool DefaultValue;

        [BoxGroup("Value"), ReadOnly, AllowNesting]
        public bool Value;

        [ShowNativeProperty]
        public SettingType Type => SettingType.Boolean;


        public void SetValue(bool value)
        {
            Value = value;
        }

        public void Reset()
        {
            Value = DefaultValue;
        }
    }
}
