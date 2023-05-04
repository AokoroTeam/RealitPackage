using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Reflection;
using static UnityEditor.Experimental.GraphView.Port;
using System.Collections.ObjectModel;

namespace LTX.ChanneledProperties
{
    

    [System.Serializable]
    public class ChanneledProperty<T>
    {
        public event Action<T> OnValueChanged;

        //Properties
        public int ChannelCount => _channelCount;
        
        private Channel<T> MainChannel
        {
            get
            {
                if (_needsRefresh)
                    FindMainChannel();

                return HasMainChannel ? channels[keyPointers[MainChannelKey]] : default;
            }
        }

        private ChannelKey MainChannelKey
        {
            get
            {
                if (_needsRefresh)
                    FindMainChannel();

                return HasMainChannel ? _mainChannelKey : default;
            }
        }
        public bool HasMainChannel
        {
            get
            {
                if (_needsRefresh)
                    FindMainChannel();

                return _hasMainChannel;
            }
        }

        public T Value => HasMainChannel ? MainChannel.Value : _defaultValue;

        [SerializeField]
        private Channel<T>[] channels;
        [SerializeField]
        private Dictionary<ChannelKey, int> keyPointers;
        [SerializeField]
        private bool[] availableSlots;
        [SerializeField]
        private ChannelKey _mainChannelKey;
        [SerializeField]
        private bool _hasMainChannel;
        [SerializeField]
        private bool _needsRefresh;
        [SerializeField]
        private T _defaultValue;
        [SerializeField]
        private int _channelCount;
        [SerializeField]
        private int _capacity;
        [SerializeField]
        private bool _expandOnFullCapacityReached;

#region Constructors
        public ChanneledProperty() : this(default, 16, false) { }
        public ChanneledProperty(T defaultValue) : this(defaultValue, 16, false) { }
        public ChanneledProperty(T defaultValue, int capacity) : this(defaultValue, capacity, false) { }
        public ChanneledProperty(T defaultValue, bool expandOnFullCapacityReached) : this(defaultValue, 16, expandOnFullCapacityReached ) { }
        
        public ChanneledProperty(T defaultValue, int capacity, bool expandOnFullCapacityReached)
        {
            channels = new Channel<T>[capacity];
            availableSlots = new bool[capacity];
            keyPointers = new Dictionary<ChannelKey, int>(capacity);
            

            for (int i = 0; i < capacity; i++)
            {
                availableSlots[i] = true;
                channels[i] = new Channel<T>(-10, default(T), ChannelKey.None);
            }

            this._needsRefresh = true;
            this._defaultValue = defaultValue;
            this._capacity = capacity;
            this._expandOnFullCapacityReached = expandOnFullCapacityReached;
        }


#endregion
        public T this[ChannelKey key] => GetValueFrom(key);

        public void AddChannel(ChannelKey key) => AddChannel(key, PriorityTags.None, _defaultValue);
        public void AddChannel(ChannelKey key, int priority) => AddChannel(key, priority, _defaultValue);
        public void AddChannel(ChannelKey key, PriorityTags priority) => AddChannel(key, ChannelUtility.PriorityToInt(priority), _defaultValue);
        public void AddChannel(ChannelKey key, PriorityTags priority, T value) => AddChannel(key, ChannelUtility.PriorityToInt(priority), value);

        public void AddChannel(ChannelKey key, int priority, T value)
        {
            if (_channelCount >= _capacity)
            {
                //If here, then the channeled property has reached max capacity
                if (_expandOnFullCapacityReached)
                    ExpandChannelsBuffer();
                else
                {
                    Debug.LogError($"Couldn't add channel. ChanneledProperty has reached maximum size. Consider changing the capacity or reducing the concurrent usage.");
                    return;
                }
            }
            int lastMainPriority = HasMainChannel ? MainChannel.Priority : int.MinValue;

            for (int i = 0; i < _capacity; i++)
            {
                if (availableSlots[i])
                {
                    availableSlots[i] = false;

                    channels[i].Priority = priority;
                    channels[i].Value = value;
                    channels[i].Key = key;

                    keyPointers.Add(key, i);
                    _channelCount++;

                    if (lastMainPriority <= priority)
                        FindMainChannel();
                    else
                        this._needsRefresh = true;

                    break;
                }
            }
        }

        /// <summary>
        /// Removes completly a channel
        /// </summary>
        /// <param name="key">Key of the channel</param>
        /// <returns>True if the channel existed</returns>
        public bool RemoveChannel(ChannelKey key)
        {
            if (HasChannel(key))
            {                
                int index = keyPointers[key];
                var lastMainChannelKey = MainChannelKey;

                channels[index] = Channel<T>.Empty;
                availableSlots[index] = true;
                keyPointers.Remove(key);
                
                _channelCount--;

                if (lastMainChannelKey._id == key._id)
                    FindMainChannel();

                return true;
            }

            return false;
        }

        private void ExpandChannelsBuffer()
        {
            int newCapacity = this._capacity * 2;

            bool[] newAvaiableSlots = new bool[_capacity];
            Channel<T>[] newChannels = new Channel<T>[_capacity];

            for (int i = 0; i < newCapacity; i++)
            {
                newAvaiableSlots[i] = i < _capacity ? availableSlots[i] : true;
                newChannels[i] = i < _capacity ? channels[i] : Channel<T>.Empty;
            }

            availableSlots = newAvaiableSlots;
            channels = newChannels;
        }

        /// <summary>
        /// Write a value into a channel.
        /// </summary>
        /// <param name="key">Key of the channel</param>
        /// <param name="value">New value</param>
        /// <returns>True if the channel existed and value was successfuly writed</returns>
        public bool Write(ChannelKey key, T value)
        {
            if (TryGetChannel(key, out Channel<T> channel))
            {
                channel.Value = value;

                //Updating struct value inside dictionnary
                channels[keyPointers[key]] = channel;

                //If main channel was changed
                if(HasMainChannel && MainChannelKey.Equals(key))
                    NotifyValueChange();

                return true;
            }

            return false;
        }

        /// <summary>
        /// Set a new priority for a channel without erasing it.
        /// </summary>
        /// <param name="key">Key of the channel</param>
        /// <param name="newPriority">New priority</param>
        /// <returns>True if the channel existed and was successfuly modified</returns>
        public bool ChangeChannelPriority(ChannelKey key, int newPriority)
        {
            if (TryGetChannel(key, out Channel<T> channel))
            {
                int mainPriority = -1;

                if (HasMainChannel)
                    mainPriority = MainChannel.Priority;

                channel.Priority = newPriority;

                //Updating channel inside dictionnary
                channels[keyPointers[key]] = channel;

                if (IsMainChannel(key) || newPriority > 0 && newPriority > mainPriority)
                    FindMainChannel();

                return true;
            }

            return false;
        }

        /// <summary>
        /// Set a new priority for a channel without erasing it.
        /// </summary>
        /// <param name="key">Key of the channel</param>
        /// <param name="newPriority">New priority</param>
        /// <returns>True if the channel existed and was successfuly modified</returns>
        public bool ChangeChannelPriority(ChannelKey key, PriorityTags newPriority) => ChangeChannelPriority(key, ChannelUtility.PriorityToInt(newPriority));

        /// <summary>
        /// Remove all channels
        /// </summary>
        public void Clear()
        {
            keyPointers.Clear();
            
            for(int i = 0; i < _capacity; i++)
            {
                availableSlots[i] = true;
                channels[i] = default;
            }

            _mainChannelKey = default;
            _hasMainChannel = false;
            _needsRefresh = true;
            _channelCount = 0;

            NotifyValueChange();
        }

        /// <summary>
        /// Get the channel of a key if he's in charge of it.
        /// </summary>
        /// <param name="key">Key of a channel</param>
        /// <param name="channel">Output. Default if not found.</param>
        /// <returns>True if a channel is found</returns>
        public bool TryGetChannel(ChannelKey key, out Channel<T> channel)
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

        /// <summary>
        /// Faster way to get value from key but key needs to exists
        /// </summary>
        /// <param name="key">Key of a channel</param>
        /// <returns>Value of channel</returns>
        private T GetValueFrom(ChannelKey key)
        {
            if (TryGetChannel(key, out Channel<T> channel))
                return channel.Value;

            return default(T);
        }

        /// <summary>
        /// Does this key is in charge of the main channel?
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool HasChannel(ChannelKey key) => HasMainChannel && keyPointers.ContainsKey(key);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool IsMainChannel(ChannelKey key) => HasMainChannel && MainChannelKey._id == key._id;


        /// <summary>
        /// Go through all channels and find the one in control
        /// </summary>
        private void FindMainChannel()
        {
            //Not dirty anymore because the new value is re-evaluated.
            this._needsRefresh = false;
            bool hasMainChannel = this._hasMainChannel;
            bool lastHasMainChannel = this._hasMainChannel;
            
            if (this.ChannelCount == 0)
            {
                _hasMainChannel = false;
                _mainChannelKey = default;

                //Force notify because the new value is default value
                if (lastHasMainChannel)
                    NotifyValueChange();
                return;
            }


            ChannelKey mainChannelKey = this._mainChannelKey;
            ChannelKey lastMainChannelKey = this._mainChannelKey;

            int highestPriority = -1;

            for (int i = 0; i < this._capacity; i++)
            {
                if (availableSlots[i])
                    continue;

                Channel<T> channel = channels[i];

                int priority = channel.Priority;
                if (priority > highestPriority)
                {
                    highestPriority = priority;
                    mainChannelKey = channel.Key;
                    hasMainChannel = true;
                }
            }

            //Channels with priority set to none can never be in control.
            //If all channels are set to none, then the property returns the default value.
            if (hasMainChannel)
            {
                this._hasMainChannel = hasMainChannel;
                this._mainChannelKey = mainChannelKey;
            }
            else
            {
                this._hasMainChannel = false;
            }

            if (lastHasMainChannel != hasMainChannel || lastMainChannelKey._id != mainChannelKey._id)
                NotifyValueChange();
            
        }

        private void NotifyValueChange()
        {
            OnValueChanged?.Invoke(Value);
        }

        public static implicit operator T(ChanneledProperty<T> interaction) => interaction.Value;

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}


