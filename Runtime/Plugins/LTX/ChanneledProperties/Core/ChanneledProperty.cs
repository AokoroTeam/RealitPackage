using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Reflection;

namespace LTX.ChanneledProperties
{
    public enum PriorityTags
    {
        None = -1,
        Smallest = 1,
        VerySmall = 3,
        Small = 5,
        Default = 10,
        High = 25,
        VeryHigh = 50,
        Highest = 100
    }

    

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

                return HasMainChannel ? channels[MainChannelKey] : default;
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

        public T Value
        {
            get
            {
#if UNITY_EDITOR
                T value = HasMainChannel ? MainChannel.Value : _defaultValue;
                _value = value;
                return value;
#else
                return HasMainChannel ? MainChannel.Value : _defaultValue
#endif
            }
        }

#if UNITY_EDITOR
        [SerializeField]
        private T _value;
#endif
        [SerializeField]
        private Dictionary<ChannelKey, Channel<T>> channels;

        [SerializeField]
        private Dictionary<object, Channel<T>> keyOwners;

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


        private int _internalKeyCount;

        public ChanneledProperty() : this(default) { }
        
        public ChanneledProperty(T defaultValue, int capacity = 16)
        {
            this._needsRefresh = true;
            
            channels = new(capacity);
            this._defaultValue = defaultValue;
        }


        public T this[ChannelKey key] => GetValueFrom(key);



        public void AddChannel(ChannelKey key, int priority)
            => AddChannel(key, priority, _defaultValue);
        public void AddChannel(ChannelKey key, PriorityTags priority)
            => AddChannel(key, (int)priority, _defaultValue);
        public void AddChannel(ChannelKey key, PriorityTags priority, T value)
            => AddChannel(key, (int)priority, value);

        public void AddChannel(ChannelKey key, int priority, T value)
        {
            Channel<T> channel = new(priority, value);

            int lastMainPriority = HasMainChannel ? MainChannel.Priority : int.MinValue;

            channels.Add(key, channel);
            _channelCount = channels.Count;

            if (lastMainPriority <= priority)
                FindMainChannel();
            else
                this._needsRefresh = true;
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
                var lastMainChannelKey = MainChannelKey;
                channels.Remove(key);
                _channelCount = channels.Count;

                if (lastMainChannelKey.Equals(key))
                    FindMainChannel();

                return true;
            }

            return false;
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
                channels[key] = channel;

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
                channels[key] = channel;

                if (newPriority > 0 && newPriority > mainPriority)
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
        public bool ChangeChannelPriority(ChannelKey key, PriorityTags newPriority) => ChangeChannelPriority(key, (int)newPriority);

        /// <summary>
        /// Remove all channels
        /// </summary>
        public void Clear()
        {
            channels.Clear();

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
            if (channels.TryGetValue(key, out channel))
                return true;
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
        public bool HasChannel(ChannelKey key) => HasMainChannel && channels.ContainsKey(key);


        /// <summary>
        /// Go through all channels and find the one in control
        /// </summary>
        private void FindMainChannel()
        {
            //Not dirty anymore because the new value is re-evaluated.
            this._needsRefresh = false;
            if (this.ChannelCount == 0)
            {
                _hasMainChannel = false;
                _mainChannelKey = default;

                return;
            }

            ///Debug
            //var watch = new System.Diagnostics.Stopwatch();
            //watch.Start();

            bool hasMainChannel = this._hasMainChannel;
            ChannelKey mainChannelKey = this._mainChannelKey;

            int highestPriority = -1;

            foreach (var kvp in channels)
            {
                var channel = kvp.Value;
                var channelKey = kvp.Key;

                if (channel.Priority > highestPriority)
                {
                    mainChannelKey = channelKey;
                    hasMainChannel = true;
                }
            }
            ///Debug
            //watch.Stop();
            //Debug.Log($"Going through channels has taken {watch.ElapsedMilliseconds} which is {watch.ElapsedMilliseconds / (float)this.ChannelCount} ms per channel");
            
            
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

            if(this._hasMainChannel != hasMainChannel || !this._mainChannelKey.Equals(mainChannelKey))
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


