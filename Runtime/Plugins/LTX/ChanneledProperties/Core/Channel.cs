using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LTX.ChanneledProperties
{
    [System.Serializable]
    public struct Channel<T>
    {
        public static Channel<T> Empty => _empty;
        private static Channel<T> _empty => new Channel<T>();


        public int Priority { 
            get => _priority;
            set => _priority = value;
        }

        public T Value { 
            get => _value;
            set => _value = value;
        }

        [SerializeField]
        private int _priority;

        [SerializeField]
        private T _value;

        public Channel(int Priority, T Value)
        {
            this._priority = Priority;
            this._value = Value;
        }

        public Channel(PriorityTags Priority, T Value) : this(ChannelUtility.PriorityToInt(Priority), Value) { }
    }
}
