using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Aokoro
{
    [System.Serializable]
    public class Channel<T>
    {
        public object Owner 
        { 
            get => _owner;
            private set
            {
                _owner = value;
#if UNITY_EDITOR
                ownerRef = value as UnityEngine.Object;
#endif
            }
        }
        
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

        private object _owner;

#if UNITY_EDITOR
        [SerializeField]
        private UnityEngine.Object ownerRef;
#endif
        public Channel(int Priority, object owner, T Value)
        {
            this._priority = Priority;
            this._owner = owner;
            this._value = Value;
#if UNITY_EDITOR
            this.ownerRef = owner as UnityEngine.Object;
#endif
        }
        public Channel(PriorityTags Priority, object owner, T Value) : this((int)Priority, owner, Value) { }

        public void Set(T Value) => this.Value = Value;

        public void ChangePriority(int priority) => this.Priority = priority;
        public void ChangePriority(PriorityTags priority) => this.Priority = (int)priority;
    }
}
