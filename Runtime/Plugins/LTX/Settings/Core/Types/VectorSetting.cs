using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LTX.Settings.Types
{
    [System.Serializable]
    public struct Vector3Setting : ISetting<Vector3>
    {
        #region Interface
        Vector3 ISetting<Vector3>.Value => Value;
        Vector3 ISetting<Vector3>.DefaultValue => DefaultValue;
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
        public Vector3 DefaultValue;
        [BoxGroup("Value"), ReadOnly, AllowNesting]
        public Vector3 Value;

        public SettingType Type => SettingType.Vector3;

        public void SetValue(Vector3 value)
        {
            Value = value;
        }
        public void Reset()
        {
            Value = DefaultValue;
        }
    }

    [System.Serializable]
    public struct Vector2Setting : ISetting<Vector2>
    {
        #region Interface
        Vector2 ISetting<Vector2>.Value => Value;
        Vector2 ISetting<Vector2>.DefaultValue => DefaultValue;
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
        public Vector2 DefaultValue;
        [BoxGroup("Value"), ReadOnly, AllowNesting]
        public Vector2 Value;

        public SettingType Type => SettingType.Vector2;

        public void SetValue(Vector2 value)
        {
            Value = value;
        }
        public void Reset()
        {
            Value = DefaultValue;
        }
    }
}
