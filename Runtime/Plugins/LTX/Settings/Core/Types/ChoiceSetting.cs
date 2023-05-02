using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LTX.Settings
{
    public struct ChoiceSetting : ISetting<int>
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
        [BoxGroup("Value")]
        public string[] Choices;

        [BoxGroup("Value"), ReadOnly, AllowNesting]
        public int Value;

        public SettingType Type => SettingType.Choice;

        public string GetChoice(int index) => Choices[index];
        public string GetCurrentChoice() => GetChoice(Value);

        public void SetValue(int value)
        {
            Value = Mathf.Clamp(value, 0, Choices.Length - 1);
        }
        public void Reset()
        {
            Value = DefaultValue;
        }
    }
}
