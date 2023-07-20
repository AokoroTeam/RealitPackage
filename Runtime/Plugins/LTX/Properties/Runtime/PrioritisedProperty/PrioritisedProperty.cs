using UnityEngine;

namespace LTX.ChanneledProperties
{

    /// <summary>
    /// A property that returns the value in the channel with the highest priority.
    /// </summary>
    /// <typeparam name="T">Type of value returned</typeparam>
    [System.Serializable]
    public class PrioritisedProperty<T> : ChanneledProperty<T,PriorityChannel<T>>
    {
        private PriorityChannel<T> MainChannel
        {
            get
            {
                if (_needsRefresh)
                    FindMainChannel();

                return HasMainChannel ? channels[GetChannelIndex(MainChannelKey)] : default;
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

        public override T Value => HasMainChannel ? MainChannel.Value : _defaultValue;

       
        [SerializeField, HideInInspector]
        private ChannelKey _mainChannelKey;
        [SerializeField, HideInInspector]
        private bool _hasMainChannel;
        [SerializeField, HideInInspector]
        private bool _needsRefresh;
        [SerializeField, HideInInspector]
        private T _defaultValue;
       
        #region Constructors

        private PrioritisedProperty() : this(default, 16, false) { }

        public PrioritisedProperty(T defaultValue = default, int capacity = 16, bool expandOnFullCapacityReached = false) : base(capacity, expandOnFullCapacityReached)
        {
            this._needsRefresh = true;
            this._defaultValue = defaultValue;
        }

        #endregion
        protected override PriorityChannel<T> CreateEmptyChannel() => new(-10, default(T), ChannelKey.None);

        public void AddChannel(ChannelKey key) => AddChannel(key, PriorityTags.None, _defaultValue);
        public void AddChannel(ChannelKey key, int priority) => AddChannel(key, priority, _defaultValue);
        public void AddChannel(ChannelKey key, PriorityTags priority) => AddChannel(key, ChannelPriorityUtility.PriorityToInt(priority), _defaultValue);
        public void AddChannel(ChannelKey key, PriorityTags priority, T value) => AddChannel(key, ChannelPriorityUtility.PriorityToInt(priority), value);

        public void AddChannel(ChannelKey key, int priority, T value)
        {
            int lastMainPriority = HasMainChannel ? MainChannel.Priority : int.MinValue;
            if(Internal_AddChannel(key, value))
            {
                ChangeChannelPriority(key, priority);

                if (lastMainPriority <= priority)
                    FindMainChannel();
                else
                    this._needsRefresh = true;
            }
        }

        /// <summary>
        /// Removes completly a channel
        /// </summary>
        /// <param name="key">Key of the channel</param>
        /// <returns>True if the channel existed</returns>
        public override bool RemoveChannel(ChannelKey key)
        {
            var lastMainChannelKey = MainChannelKey;
            bool output = base.RemoveChannel(key);
            
            if (output)
            {
                if (lastMainChannelKey._id == key._id)
                    FindMainChannel();
            }

            return output;
        }

        /// <summary>
        /// Write a value into a channel.
        /// </summary>
        /// <param name="key">Key of the channel</param>
        /// <param name="value">New value</param>
        /// <returns>True if the channel existed and value was successfuly writed</returns>
        public override bool Write(ChannelKey key, T value)
        {
            if(base.Write(key, value))
            {
                //If main channel was changed
                if (HasMainChannel && MainChannelKey._id == key._id)
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
            if (TryGetChannel(key, out PriorityChannel<T> channel))
            {
                int mainPriority = -1;

                if (HasMainChannel)
                    mainPriority = MainChannel.Priority;

                channel.Priority = newPriority;

                //Updating channel inside dictionnary
                channels[GetChannelIndex(key)] = channel;

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
        public bool ChangeChannelPriority(ChannelKey key, PriorityTags newPriority) => ChangeChannelPriority(key, ChannelPriorityUtility.PriorityToInt(newPriority));

        /// <summary>
        /// Remove all channels
        /// </summary>
        public override void Clear()
        {
            base.Clear();
            _mainChannelKey = default;
            _hasMainChannel = false;
            _needsRefresh = true;

            NotifyValueChange();
        }

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

            bool hasNewMainChannel = false;
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
            int iteration = 0;
            int index = 0;

            while (iteration < ChannelCount)
            {
                //Skipping unused channels
                if (IsChannelAvailable(index))
                {
                    index++;
                    continue;
                }

                PriorityChannel<T> channel = channels[index];
                iteration++;

                int priority = channel.Priority;
                if (priority > highestPriority)
                {
                    highestPriority = priority;
                    mainChannelKey = channel.Key;
                    hasNewMainChannel = true;
                }

                index++;
            }

            //Channels with priority set to none can never be in control.
            //If all channels are set to none, then the property returns the default value.
            if (hasNewMainChannel)
            {
                this._hasMainChannel = hasNewMainChannel;
                this._mainChannelKey = mainChannelKey;
            }
            else
            {
                this._hasMainChannel = false;
            }

            if (lastHasMainChannel != hasNewMainChannel || lastMainChannelKey._id != mainChannelKey._id)
                NotifyValueChange();
            
        }

    }
}


