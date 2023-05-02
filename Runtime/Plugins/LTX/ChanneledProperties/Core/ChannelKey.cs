using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LTX.ChanneledProperties
{
    [System.Serializable]
    public struct ChannelKey
    {
#region Static
        private static Dictionary<Object, ChannelKey> _createdKeys;

        private static long _internalChannelKeyCount = 0;


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        public static void InitializeOnLoad()
        {
            Debug.Log($"[Channeled properties] Initializing channelkeys...");
            _createdKeys = new Dictionary<Object, ChannelKey>();

            SceneManager.sceneLoaded += SceneManager_sceneLoaded;
            SceneManager.sceneUnloaded += SceneManager_sceneUnloaded; ;
        }

        private static void SceneManager_sceneUnloaded(Scene scene)
        {
            CleanMissingReferences();
        }
        private static void SceneManager_sceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            CleanMissingReferences();
        }

        private static void CleanMissingReferences()
        {

            Debug.Log($"[Channeled properties] Clearing missing channelkeys...");
            Dictionary<Object, ChannelKey> _createdKeys = new();

            foreach (var item in ChannelKey._createdKeys)
            {
                if (item.Key != null)
                    _createdKeys.Add(item.Key, item.Value);
            }

            ChannelKey._createdKeys = _createdKeys;
        }

        
        public static ChannelKey GetUniqueChannelKey()
        {
            _internalChannelKeyCount++;
            ChannelKey channelKey = new ChannelKey(_internalChannelKeyCount);

            return channelKey;
        }

        public static ChannelKey GetUniqueChannelKey(Object pointer)
        {
            var channelKey = GetUniqueChannelKey();
#if UNITY_EDITOR
            channelKey.pointer = pointer;
#endif
            return channelKey;
        }

#endregion



        [SerializeField]
        private long _id;

#if UNITY_EDITOR
        [SerializeField]
        private Object pointer;
#endif
        private ChannelKey(long id)
        {
            _id = id;
#if UNITY_EDITOR
            pointer = null;
#endif
        }

        public static implicit operator ChannelKey(Object unityObject)
        {
            if(_createdKeys.TryGetValue(unityObject, out ChannelKey channelKey))
                return channelKey;
            else
                return GetUniqueChannelKey(unityObject);
        }
        public override int GetHashCode() => _id.GetHashCode();

        public override bool Equals(object obj)
        {
            if(obj is ChannelKey key)
                return key._id == _id;

            return base.Equals(obj);
        }


    }
}
