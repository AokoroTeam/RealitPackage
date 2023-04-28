using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aokoro.Settings
{
    public enum SettingType
    {
        Boolean,
        Float,
        Integer,
        Text,
        Vector2,
        Vector3,
        Choice,
    }

    public interface ISetting 
    {
        public string Label { get; }
        public string InternalName { get; }
        public string Infos { get; }
        public SettingType Type { get; }
        void Reset();
    }

    public interface ISetting<T> : ISetting
    {
        public  T Value { get; }
        public  T DefaultValue { get; }
        void SetValue(T value);
    }
}
