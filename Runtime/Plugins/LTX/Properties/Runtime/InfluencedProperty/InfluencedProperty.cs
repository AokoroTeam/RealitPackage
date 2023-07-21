using System;
using System.Collections.Generic;
using UnityEngine;

namespace LTX.ChanneledProperties
{
    internal enum Operation
    {
        Multiply,
        Divide,
        Add,
        Subtract,
    }

   
    [Flags]
    public enum Influence
    {
        /// <summary>
        /// Useless
        /// Value = Value
        /// </summary>
        None = 0,
        /// <summary>
        /// Value = Value * X
        /// </summary>
        Multiply = 1,
        /// <summary>
        /// Value = Value / X
        /// </summary>
        Divide = 2,
        /// <summary>
        /// Value = Value + X
        /// </summary>
        Add = 4,
        /// <summary>
        /// Value = Value - X
        /// </summary>
        Subtract = 8,
        //Only used in add or substract.
        /// <summary>
        /// Use with add or substract :
        /// Value = Value + or - (StartValue * X)
        /// </summary>
        MultipleOfStartValue = 16,
        /// <summary>
        /// Use with add or substract :
        /// Value = Value + or - (LayerValue * X)
        /// </summary>
        MultipleOfLayerValue = 32,
        /// <summary>
        /// Use with add or substract :
        /// Value = Value + or - (Value * X)
        /// /// </summary>
        MultipleOfCurrentValue = 64
    }

    /// <summary>
    /// A property with a base value that gets modified by all channels.
    /// Channels are seperated in layers and ordered in them si you can group some operations and emulate parentheses.
    /// </summary>
    /// <typeparam name="T">Type of value returned</typeparam>
    [System.Serializable]
    public class InfluencedProperty<T> : ChanneledProperty<T, InfluencedChannel<T>>
    {
        public override T Value
        {
            get
            {
                if(NeedsRefresh)
                {
                    _value = CalculateValue();
                    NeedsRefresh = false;
                }

                return _value;
            }
        }

        private bool NeedsRefresh
        {
            get => _needsRefresh;
            set
            {
                _needsRefresh = value;
                if (value)
                    NotifyValueChange();
            }
        }

        protected override InfluencedChannel<T> CreateEmptyChannel() => new(default, ChannelKey.None, Influence.None, -1, -1);

        [SerializeField]
        private T _value;
        [SerializeField, HideInInspector]
        private T _startValue;


        private bool _needsRefresh;

        private Dictionary<int, List<InfluencedChannel<T>>> orderedChannelsBuffer;
        private List<int> layersBuffer;

        private InfluencedProperty() : this(default, 16, false) { }

        public InfluencedProperty(T startValue, int capacity = 16, bool expandOnFullCapacityReached = false) : base(capacity, expandOnFullCapacityReached)
        {
            this.NeedsRefresh = true;
            this._startValue = startValue;
        }
        protected override void SetupChannels(int capacity)
        {
            base.SetupChannels(capacity);
            orderedChannelsBuffer = new (capacity);
            layersBuffer = new();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key">Owner of the channel</param>
        /// <param name="value">Value of the channel (how mush it modifies the entering value)</param>
        /// <param name="influence">Type of operation it applies</param>
        /// <param name="layer"></param>
        /// <param name="orderInLayer"></param>
        public void AddChannel(ChannelKey key, T value, Influence influence = Influence.Add, int layer = 0, int orderInLayer = 0)
        {
            InfluencedChannel<T> channelToAdd = new(value, key, influence, layer, orderInLayer);
            Internal_AddChannel(channelToAdd);
            NeedsRefresh = true;
        }

        public override bool RemoveChannel(ChannelKey key)
        {
            bool result = base.RemoveChannel(key);
            if (result)
                NeedsRefresh = true;

            return result;
        }

        public override bool Write(ChannelKey key, T value)
        {
            bool result = base.Write(key, value);
            if(result)
                NeedsRefresh = true;

            return result;

        }

        public bool ChangeInfluence(ChannelKey key, Influence influence)
        {
            
            if (TryGetChannel(key, out InfluencedChannel<T> channel))
            {
                channel.influence = influence;
                channels[GetChannelIndex(key)] = channel;
                NeedsRefresh = true;
                return true;
            }

            return false;
        }

        public bool ChangeLayer(ChannelKey key, int layer)
        {
            
            if (TryGetChannel(key, out InfluencedChannel<T> channel))
            {
                channel.layer = layer;
                channels[GetChannelIndex(key)] = channel;
                NeedsRefresh = true;
                return true;
            }

            return false;
        }
        public bool ChangeOrderInLayer(ChannelKey key, int orderInLayer)
        {
            
            if (TryGetChannel(key, out InfluencedChannel<T> channel))
            {
                channel.orderInLayer = orderInLayer;
                channels[GetChannelIndex(key)] = channel;
                NeedsRefresh = true;
                return true;
            }

            return false;
        }

        private T CalculateValue()
        {
            T currentLayerValue = _startValue;
            T currentValue = _startValue;

            orderedChannelsBuffer.Clear();
            layersBuffer.Clear();

            for (int i = 0; i < channels.Length; i++)
            {
                if (IsChannelAvailable(i))
                    continue;

                InfluencedChannel<T> channel = channels[i];
                if (orderedChannelsBuffer.ContainsKey(channel.layer))
                    orderedChannelsBuffer[channel.layer].Add(channel);
                else
                    orderedChannelsBuffer.Add(channel.layer, new() { channel });

                if (!layersBuffer.Contains(channel.layer))
                    layersBuffer.Add(channel.layer);
            }

            //Tri par layer
            layersBuffer.Sort((x, y) => x.CompareTo(y));
            foreach (var layer in layersBuffer)
            {
                List<InfluencedChannel<T>> channelList = orderedChannelsBuffer[layer];

                channelList.Sort((x, y) => x.orderInLayer.CompareTo(y.orderInLayer));

                foreach (var channel in channelList)
                {
                    //
                    T output = currentValue;
                    Influence influence = channel.influence;

                    //Add or substract first because they are the most complex
                    if (influence.HasFlag(Influence.Add) || influence.HasFlag(Influence.Subtract))
                    {
                        bool isMultipleOf = true;
                        T baseValue = default;

                        if (influence.HasFlag(Influence.MultipleOfCurrentValue))
                            baseValue = currentValue;
                        else if (influence.HasFlag(Influence.MultipleOfLayerValue))
                            baseValue = currentLayerValue;
                        else if (influence.HasFlag(Influence.MultipleOfStartValue))
                            baseValue = _startValue;
                        else
                            isMultipleOf = false;


                        T addedValue;
                        //Set added value if is a multiple of something
                        if (isMultipleOf)
                            addedValue = InfluencedPropertyUtilities.DoOperation(baseValue, channel._value, Operation.Multiply);
                        else
                            addedValue = channel._value;

                        //Adds or remove the correct value
                        if (influence.HasFlag(Influence.Add))
                            output = InfluencedPropertyUtilities.DoOperation(currentValue, addedValue, Operation.Add);
                        else
                            output = InfluencedPropertyUtilities.DoOperation(currentValue, addedValue, Operation.Subtract);

                    }
                    else
                    {
                        switch (influence)
                        {
                            case Influence.None:
                                break;
                            case Influence.Multiply:
                                output = InfluencedPropertyUtilities.DoOperation(currentValue, channel._value, Operation.Multiply);
                                break;
                            case Influence.Divide:
                                output = InfluencedPropertyUtilities.DoOperation(currentValue, channel._value, Operation.Divide);
                                break;
                        }
                    }

                    currentValue = output;
                }

                currentLayerValue = currentValue;
            }

            return currentValue;
        }

    }
}


