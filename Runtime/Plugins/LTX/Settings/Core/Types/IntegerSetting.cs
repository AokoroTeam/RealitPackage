using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace LTX.Settings.Types
{
    [System.Serializable]
    public struct IntegerSetting : ISetting<int>
    {
        #region Interface
        int ISetting<int>.Value => Value;
        int ISetting<int>.DefaultValue => DefaultValue;
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
        public int DefaultValue;

        [BoxGroup("Value"), ReadOnly, AllowNesting]
        public int Value;

        public Vector2Int MinMax;
        public SettingType Type => SettingType.Integer;

        public void SetValue(int value)
        {
            Value = Mathf.Clamp(value, MinMax.x, MinMax.y);
        }
        public void Reset()
        {
            Value = DefaultValue;
        }
    }
}
