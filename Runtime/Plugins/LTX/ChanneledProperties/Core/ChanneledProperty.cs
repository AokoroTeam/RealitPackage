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
        public int ChannelCount => channels.Count;
        public bool HasMainChannel => MainChannelIndex != -1 && ChannelCount > 0;

        private Channel<T> MainChannel
        {
            get
            {
                if (_channelsAreDirty)
                    FindMainChannel();

                return HasMainChannel ? channels[MainChannelIndex] : default;
            }
        }

        private int MainChannelIndex
        {
            get
            {
                if (_channelsAreDirty)
                    FindMainChannel();

                return _mainChannelIndex;
            }
        }

        public object MainChannelOwner => MainChannel.Owner;

        public T Value
        {
            get
            {
                T value = ChannelCount != 0 && HasMainChannel ? MainChannel.Value : _defaultValue;
#if UNITY_EDITOR
                _value = value;
#endif
                return value;
            }
        }

#if UNITY_EDITOR
        [SerializeField]
        private T _value;
#endif
        [SerializeField]
        private List<Channel<T>> channels;

        [SerializeField]
        private int _mainChannelIndex;
        [SerializeField]
        private bool _channelsAreDirty;
        [SerializeField]
        private T _defaultValue;


        public ChanneledProperty() : this(default) { }
        
        public ChanneledProperty(T defaultValue)
        {
            this._channelsAreDirty = true;
            
            channels = new List<Channel<T>>();
            this._defaultValue = defaultValue;
        }


        public T this[object owner] => GetValueFromOwner(owner);



        public void AddChannel(object owner, int priority) => AddChannel(owner, priority, _defaultValue);
        public void AddChannel(object owner, PriorityTags priority) => AddChannel(owner, (int)priority, _defaultValue);
        public void AddChannel(object owner, PriorityTags priority, T value) => AddChannel(owner, (int)priority, value);
        public void AddChannel(object owner, int priority, T value)
        {
            Channel<T> channel = new(priority, owner, value);

            if (TryGetChannelIndex(owner, out int index))
                channels[index] = channel;
            else
            {
                int lastMainPriority = HasMainChannel ? MainChannel.Priority : int.MinValue;
                channels.Add(channel);

                this._channelsAreDirty = true;
                if (lastMainPriority <= priority)
                    NotifyValueChange();
            }
        }
        /// <summary>
        /// Removes completly a channel
        /// </summary>
        /// <param name="owner">Owner of the channel</param>
        /// <returns>True if the channel existed</returns>
        public bool RemoveChannel(object owner)
        {
            if (TryGetChannelIndex(owner, out int index))
            {                
                int lastMainChannelIndex = MainChannelIndex;
                channels.RemoveAt(index);

                this._channelsAreDirty = true;
                if (lastMainChannelIndex == index)
                    NotifyValueChange();

                return true;
            }

            return false;
        }

        /// <summary>
        /// Write a value into a channel.
        /// </summary>
        /// <param name="owner">Owner of the channel</param>
        /// <param name="value">New value</param>
        /// <returns>True if the channel existed and value was successfuly writed</returns>
        public bool Write(object owner, T value)
        {
            if (TryGetChannelIndex(owner, out int index))
            {
                int lastMainChannelIndex = MainChannelIndex;
                channels[index].Set(value);

                if (lastMainChannelIndex == index)
                    NotifyValueChange();

                return true;
            }

            return false;
        }
        /// <summary>
        /// Set a new priority for a channel without erasing it.
        /// </summary>
        /// <param name="owner">Owner of the channel</param>
        /// <param name="newPriority">New priority</param>
        /// <returns>True if the channel existed and was successfuly modified</returns>
        public bool ChangeChannelPriority(object owner, int newPriority)
        {
            if (TryGetChannelIndex(owner, out int index))
            {
                int mainPriority = -1;

                if (HasMainChannel)
                    mainPriority = MainChannel.Priority;

                channels[index].ChangePriority(newPriority);

                if (newPriority > 0 && newPriority > mainPriority || HasChannelOwnBy(owner))
                {
                    _channelsAreDirty = true;
                    NotifyValueChange();
                }
                return true;
            }

            return false;
        }
        /// <summary>
        /// Set a new priority for a channel without erasing it.
        /// </summary>
        /// <param name="owner">Owner of the channel</param>
        /// <param name="newPriority">New priority</param>
        /// <returns>True if the channel existed and was successfuly modified</returns>
        public bool ChangeChannelPriority(object owner, PriorityTags newPriority) => ChangeChannelPriority(owner, (int)newPriority);

        /// <summary>
        /// Remove all channels
        /// </summary>
        public void Clear()
        {
            channels.Clear();

            _mainChannelIndex = -1;
            _channelsAreDirty = true;

            NotifyValueChange();
        }

        /// <summary>
        /// Get the value of a channel if the owner is in charge of it.
        /// </summary>
        /// <param name="owner">Owner of a channel</param>
        /// <param name="value">Output. Default if not found.</param>
        /// <returns>True if a channel is found</returns>
        public bool TryGetValueFromOwner(object owner, out T value)
        {
            if (TryGetChannelIndex(owner, out int index))
            {
                value = channels[index].Value;
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }

        /// <summary>
        /// Get the channel of a owner if he's in charge of it.
        /// </summary>
        /// <param name="owner">Owner of a channel</param>
        /// <param name="channel">Output. Default if not found.</param>
        /// <returns>True if a channel is found</returns>
        public bool TryGetChannel(object owner, out Channel<T> channel)
        {
            if (TryGetChannelIndex(owner, out int index))
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
        /// Does this owner is in charge of a channel?
        /// </summary>
        /// <param name="owner"></param>
        /// <returns></returns>
        public bool OwnsChannel(object owner) => GetChannelIndex(owner) != -1;

        /// <summary>
        /// Does this owner is in charge of the main channel?
        /// </summary>
        /// <param name="owner"></param>
        /// <returns></returns>
        public bool HasChannelOwnBy(object owner) => HasMainChannel && GetChannelIndex(owner) == MainChannelIndex;


        /// <summary>
        /// Faster way to get value from owner but owner needs to exists
        /// </summary>
        /// <param name="owner">Owner of a channel</param>
        /// <returns>Value of channel</returns>
        private T GetValueFromOwner(object owner) => channels[GetChannelIndex(owner)].Value;

        /// <summary>
        /// Get the index of the channel in the global list
        /// </summary>
        /// <param name="owner">Owner of a channel<</param>
        /// <returns>Index</returns>
        private int GetChannelIndex(object owner)
        {
            foreach (var channel in channels)
            {
                if (channel.Owner == owner)
                    return channels.IndexOf(channel);
            }

            return -1;
        }

        private bool TryGetChannelIndex(object owner, out int index)
        {
            index = GetChannelIndex(owner);
            return index != -1;
        }

        /// <summary>
        /// Go through all channels and find the one in control
        /// </summary>
        private void FindMainChannel()
        {
            //Not dirty anymore because the new value is re-evaluated.
            this._channelsAreDirty = false;
            
            if (ChannelCount > 0)
            {
                int index = 0;
                //Getting the channel with the highest priority.
                //If two channels are equals, then the first (which is the oldest) is kept.
                for (int i = 1; i < ChannelCount; i++)
                {
                    if (channels[i].Priority > channels[index].Priority)
                        index = i;
                }

                //Channels with priority set to none can never be in control.
                //If all channels are set to none, then the property returns the default value.
                _mainChannelIndex = channels[index].Priority <= 0 ? -1 : index;
            }
            else
                _mainChannelIndex = -1;
        }


        private void NotifyValueChange()
        {
            var v = Value;
            OnValueChanged?.Invoke(v);
        }

        public static implicit operator T(ChanneledProperty<T> interaction) => interaction.Value;
        public override string ToString()
        {
            return Value.ToString();
        }
    }
}


