using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Realit.Core
{
    public class RealitWebgl : MonoBehaviour
    {
        private Dictionary<string, List<Action>> callbacks = new();

#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void CallOnAppReady();

        void Start()
        {
            //js callback when application is loaded
            CallOnAppReady();
        }
#endif
        public void SubscribeToEvent(string eventName, Action callback)
        {
            if(callbacks.TryGetValue(eventName, out List<Action> list))
                list.Add(callback);
            else
                callbacks.Add(eventName, new List<Action>() { callback});
        }

        public void UnsubscribeToEvent(string eventName, Action callback) 
        {
            if (callbacks.TryGetValue(eventName, out List<Action> list))
                list.Remove(callback);
        }

        public void GetMessage(string message)
        {
            Debug.Log($"[Realit] Webgl message received : {message}");
            if( callbacks.TryGetValue(message, out List<Action> list))
            {
                foreach(Action action in list)
                    action?.Invoke();
            }
        }
    }
}
