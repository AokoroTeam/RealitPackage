using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aokoro.Settings.Types
{

    [System.Serializable]
    public struct TextSetting : ISetting<string>
    {
        #region Interface
        string ISetting<string>.Value => Value;
        string ISetting<string>.DefaultValue => DefaultValue;
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
        public string DefaultValue;
        [BoxGroup("Value"), ReadOnly, AllowNesting]
        public string Value;
        public SettingType Type => SettingType.Text;

        public void SetValue(string value)
        {
            Value = value;
        }
        public void Reset()
        {
            Value = DefaultValue;
        }
    }
}
