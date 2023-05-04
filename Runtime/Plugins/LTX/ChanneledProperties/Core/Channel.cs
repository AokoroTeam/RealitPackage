using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LTX.ChanneledProperties
{
    [System.Serializable]
    public struct Channel<T>
    {
        internal static Channel<T> Empty => _empty;
        private static Channel<T> _empty => new Channel<T>(PriorityTags.None, default, ChannelKey.None);


        internal int Priority { 
            get => _priority;
            set => _priority = value;
        }

        internal T Value { 
            get => _value;
            set => _value = value;
        }

        internal ChannelKey Key
        { 
            get => _channelKey; 
            set => _channelKey = value;
        }

        [SerializeField]
        private ChannelKey _channelKey;

        [SerializeField]
        private int _priority;

        [SerializeField]
        private T _value;
        
        internal Channel(int Priority, T Value, ChannelKey channelKey)
        {
            this._priority = Priority;
            this._value = Value;
            this._channelKey = channelKey;
        }

        internal Channel(PriorityTags Priority, T Value, ChannelKey channelKey) : 
            this(ChannelUtility.PriorityToInt(Priority), Value, channelKey) { }
    }
}
