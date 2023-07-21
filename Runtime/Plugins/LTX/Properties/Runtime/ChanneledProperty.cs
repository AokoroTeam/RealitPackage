using System;
using System.Collections.Generic;
using UnityEngine;

namespace LTX.ChanneledProperties
{
    /// <summary>
    /// Base class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="C"></typeparam>
    [Serializable]
    public abstract class ChanneledProperty<T,C> : ISerializationCallbackReceiver where C : IChannel<T>
    {
        private event Action<T> _onValueChanged;

        //Properties
        public int ChannelCount => _channelCount;
        public abstract T Value { get; }

        [SerializeField]
        protected C[] channels;
        [SerializeField, HideInInspector]
        private Dictionary<ChannelKey, int> keyPointers;
        [SerializeField, HideInInspector]
        private bool[] availableSlots;
        [SerializeField, HideInInspector]
        private int _channelCount;
        [SerializeField, HideInInspector]
        protected int _capacity;
        [SerializeField, HideInInspector]
        private bool _expandOnFullCapacityReached;


        protected ChanneledProperty(int capacity = 16, bool expandOnFullCapacityReached = false)
        {
            this._capacity = capacity;
            this._expandOnFullCapacityReached = expandOnFullCapacityReached;

            SetupChannels(capacity);
        }

        public T this[ChannelKey key] => GetValueFrom(key);


        protected virtual void SetupChannels(int capacity)
        {
            channels = new C[capacity];
            availableSlots = new bool[capacity];
            keyPointers = new Dictionary<ChannelKey, int>(capacity);


            for (int i = 0; i < capacity; i++)
            {
                availableSlots[i] = true;
                channels[i] = CreateEmptyChannel();
            }
        }

        protected abstract C CreateEmptyChannel();


        protected int GetAvailableSlot()
        {
            for (int i = 0; i < _capacity; i++)
            {
                if (availableSlots[i])
                    return i;
            }

            return -1;
        }

        protected bool Internal_AddChannel(C channel)
        {
            if (!InternalCanAddChannel(channel.Key))
                return false;

            int index = GetAvailableSlot();
            if (index == -1)
                return false;

            if (availableSlots[index])
            {
                availableSlots[index] = false;

                channels[index] = channel;

                keyPointers.Add(channel.Key, index);
                _channelCount++;
                return true;
            }

            return false;
        }

        protected bool Internal_AddChannel(ChannelKey key, T value)
        {
            if(!InternalCanAddChannel(key))
                return false;

            for (int i = 0; i < _capacity; i++)
            {
                if (availableSlots[i])
                {
                    availableSlots[i] = false;

                    channels[i].Value = value;
                    channels[i].Key = key;

                    keyPointers.Add(key, i);
                    _channelCount++;
                    return true;
                }
            }

            return false;
        }

        private bool InternalCanAddChannel(ChannelKey key)
        {
            if (HasChannel(key))
                return false;

            if (_channelCount >= _capacity)
            {
                //If here, then the channeled property has reached max capacity
                if (_expandOnFullCapacityReached)
                    ExpandChannelsBuffer();
                else
                {
                    Debug.LogError($"Couldn't add channel. ChanneledProperty has reached maximum size. Consider changing the capacity or reducing the concurrent usage.");
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Removes completly a channel
        /// </summary>
        /// <param name="key">Key of the channel</param>
        /// <returns>True if the channel existed</returns>
        public virtual bool RemoveChannel(ChannelKey key)
        {
            if (HasChannel(key))
            {
                int index = keyPointers[key];

                channels[index] = CreateEmptyChannel();
                availableSlots[index] = true;
                keyPointers.Remove(key);

                _channelCount--;
                return true;
            }

            return false;
        }

        private void ExpandChannelsBuffer()
        {
            int newCapacity = this._capacity * 2;

            bool[] newAvaiableSlots = new bool[_capacity];
            IChannel<T>[] newChannels = new IChannel<T>[_capacity];

            for (int i = 0; i < newCapacity; i++)
            {
                newAvaiableSlots[i] = i < _capacity ? availableSlots[i] : true;
                newChannels[i] = i < _capacity ? channels[i] : CreateEmptyChannel();
            }

            availableSlots = newAvaiableSlots;
            channels = new C[_capacity];

            for(int i = 0;i < newCapacity; i++)
                channels[i] = (C)newChannels[i];
        }

        /// <summary>
        /// Get the channel of a key if he's in charge of it.
        /// </summary>
        /// <param name="key">Key of a channel</param>
        /// <param name="channel">Output. Default if not found.</param>
        /// <returns>True if a channel is found</returns>
        internal bool TryGetChannel(ChannelKey key, out C channel)
        {
            if (keyPointers.TryGetValue(key, out int index))
            {
                channel = channels[index];
                return true;
            }
            else
            {
                channel = default;
                return false;
            }
        }

        public T GetValue(ChannelKey key) => channels[GetChannelIndex(key)].Value;
        public bool TryGetValue(ChannelKey key, out T value)
        {
            if (TryGetChannel(key, out C channel))
            {
                value = channel.Value;
                return true;
            }

            value = default;
            return false;
        }

        /// <summary>
        /// Write a value into a channel.
        /// </summary>
        /// <param name="key">Key of the channel</param>
        /// <param name="value">New value</param>
        /// <returns>True if the channel existed and value was successfuly writed</returns>
        public virtual bool Write(ChannelKey key, T value)
        {
            if (TryGetChannel(key, out C channel))
            {
                channel.Value = value;

                //Updating struct value inside dictionnary
                int index = keyPointers[key];

                channels[index] = channel;

                return true;
            }
#if UNITY_EDITOR
            if (key.pointer != null)
                Debug.LogWarning($"Couldn't find key with id {key._id} for {key.pointer.name}", key.pointer);
            else
#endif
                Debug.LogWarning($"Couldn't find key with id {key._id}");

            return false;
        }

        /// <summary>
        /// Remove all channels
        /// </summary>
        public virtual void Clear()
        {
            keyPointers.Clear();

            for (int i = 0; i < _capacity; i++)
            {
                availableSlots[i] = true;
                channels[i] = default;
            }

            _channelCount = 0;
        }

        public void AddOnValueChangeCallback(Action<T> callback, bool callImmediate = false)
        {
            _onValueChanged += callback;
            if(callImmediate)
                callback?.Invoke(Value);
        }
        public void RemoveOnValueChangeCallback(Action<T> callback, bool callImmediate = false)
        {
            _onValueChanged -= callback;
            if(callImmediate)
                callback?.Invoke(Value);
        }

        public virtual void NotifyValueChange()
        {
            _onValueChanged?.Invoke(Value);
        }

        #region Utility
        /// <summary>
        /// Faster way to get value from key but key needs to exists
        /// </summary>
        /// <param name="key">Key of a channel</param>
        /// <returns>Value of channel</returns>
        private T GetValueFrom(ChannelKey key)
        {
            if (TryGetChannel(key, out C channel))
                return channel.Value;

            return default(T);
        }

        /// <summary>
        /// Does this key is in charge of the main channel?
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual bool HasChannel(ChannelKey key) => keyPointers.ContainsKey(key);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected int GetChannelIndex(ChannelKey key) => HasChannel(key) ? keyPointers[key] : -1;

        protected bool IsChannelAvailable(int index) => index > 0 && index <= availableSlots.Length && availableSlots[index];
        #endregion


        public static implicit operator T(ChanneledProperty<T,C> interaction) => interaction.Value;

        public override string ToString()
        {
            return Value.ToString();
        }

        #region Serialization
        public virtual void OnBeforeSerialize()
        {
            if (channels == null || channels.Length == 0)
                channels = new C[_capacity];

            if (availableSlots == null || availableSlots.Length == 0)
                availableSlots = new bool[_capacity];


        }

        public virtual void OnAfterDeserialize()
        {

        }

        #endregion

    }
}
