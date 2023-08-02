using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace LTX.Settings.Types
{
    [System.Serializable]
    public struct FloatSetting : ISetting<float>
    {
        [System.Serializable]
        public struct LimitedValueRange
        {
            public float[] values;

            public LimitedValueRange(float[] values)
            {
                this.values = values;
            }
            public float GetClosestValue(float value)
            {
                int length = values != null ? values.Length : 0;
                if (length > 0)
                {
                    int index = 0;
                    float minDelta = float.MaxValue;
                    for (int i = 0; i < length; i++)
                    {
                        var delta = Mathf.Abs(value - values[i]);
                        if (delta < minDelta)
                        {
                            minDelta = delta;
                            index = i;
                        }
                    }

                    //Debug.Log(values[index]);
                    return values[index];
                }

                return value;
            }
        }
        #region Interface
        float ISetting<float>.Value => Value;
        float ISetting<float>.DefaultValue => DefaultValue;
        string ISetting.Label => label;
        string ISetting.InternalName => internalName;
        string ISetting.Infos => infos;
        #endregion

        [BoxGroup("Infos"), AllowNesting]
        public string label;
        [BoxGroup("Infos"), AllowNesting]
        public string internalName;
        [BoxGroup("Infos"), AllowNesting]
        [ResizableTextArea]
        public string infos;

        [BoxGroup("Value"), AllowNesting]
        public float DefaultValue;
        [BoxGroup("Value"), ReadOnly, AllowNesting]
        public float Value;
        [BoxGroup("Value"), AllowNesting]
        public bool hasInfiniteValue;
        [BoxGroup("Value"), AllowNesting]
        [HideIf(nameof(hasInfiniteValue))]
        public LimitedValueRange limitedValues;
        [BoxGroup("Value"), AllowNesting]
        [ShowIf(nameof(hasInfiniteValue))]
        public Vector2 minMax;

        public SettingType Type => SettingType.Float;

        public void SetValue(float value)
        {
            if(hasInfiniteValue)
                Value = Mathf.Clamp(value, minMax.x, minMax.y);
            else
                Value = limitedValues.GetClosestValue(value);
            
        }

        public void Reset()
        {
            Value = DefaultValue;
        }
    }
}
