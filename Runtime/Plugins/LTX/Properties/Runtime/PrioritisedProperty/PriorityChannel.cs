using UnityEngine;

namespace LTX.ChanneledProperties
{
    [System.Serializable]
    public struct PriorityChannel<T> : IChannel<T>
    {
        internal int Priority
        {
            get => _priority;
            set => _priority = value;
        }

        internal T Value => _value;

        internal ChannelKey Key => _channelKey;
        
        T IChannel<T>.Value { get => _value; set => _value = value; }
        ChannelKey IChannel<T>.Key { get => _channelKey; set => _channelKey = value; }


        [SerializeField]
        private ChannelKey _channelKey;

        [SerializeField]
        private int _priority;

        [SerializeField]
        private T _value;
        
        internal PriorityChannel(int Priority, T Value, ChannelKey channelKey)
        {
            this._priority = Priority;
            this._value = Value;
            this._channelKey = channelKey;
        }

        internal PriorityChannel(PriorityTags Priority, T Value, ChannelKey channelKey) : 
            this(ChannelPriorityUtility.PriorityToInt(Priority), Value, channelKey) { }
    }
}
