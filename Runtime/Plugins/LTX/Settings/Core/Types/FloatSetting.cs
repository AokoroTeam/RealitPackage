using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace LTX.Settings.Types
{
    [System.Serializable]
    public struct FloatSetting : ISetting<float>
    {
        #region Interface
        float ISetting<float>.Value => Value;
        float ISetting<float>.DefaultValue => DefaultValue;
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
        public float DefaultValue;
        [BoxGroup("Value"), ReadOnly, AllowNesting]
        public float Value;

        public Vector2 MinMax;
        public SettingType Type => SettingType.Float;

        public void SetValue(float value)
        {
            Value = Mathf.Clamp(value, MinMax.x, MinMax.y);
        }
        public void Reset()
        {
            Value = DefaultValue;
        }
    }
}
