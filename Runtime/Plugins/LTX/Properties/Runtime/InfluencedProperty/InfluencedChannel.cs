using UnityEngine;

namespace LTX.ChanneledProperties
{
    [System.Serializable]
    public struct InfluencedChannel<T> : IChannel<T> 
    {
        internal ChannelKey Key => _channelKey;

        T IChannel<T>.Value { get => _value; set => _value = value; }
        ChannelKey IChannel<T>.Key { get => _channelKey; set => _channelKey = value; }

        

        [SerializeField]
        private ChannelKey _channelKey;
        [SerializeField]
        internal Influence influence;
        [SerializeField]
        internal T _value;
        [SerializeField]
        internal int layer;
        [SerializeField]
        internal int orderInLayer;

        internal InfluencedChannel(T Value, ChannelKey channelKey, Influence influence, int layer, int orderInLayer)
        {
            this._value = Value;
            this._channelKey = channelKey;
            this.layer = layer;
            this.orderInLayer = orderInLayer;
            this.influence = influence;
        }

    }
}
