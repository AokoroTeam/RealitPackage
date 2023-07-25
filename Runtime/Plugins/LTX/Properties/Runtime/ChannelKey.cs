using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LTX.ChanneledProperties
{
    [System.Serializable]
    public struct ChannelKey
    {
        #region Static
        internal static ChannelKey None => _None;
        private static ChannelKey _None = new ChannelKey(0);

        private static Dictionary<object, ChannelKey> _createdKeys;

        private static ulong _internalChannelKeyCount = 1;


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        public static void InitializeOnLoad()
        {
            Debug.Log($"[Channeled properties] Initializing channelkeys...");
            _createdKeys = new();

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
            Dictionary<object, ChannelKey> _createdKeys = new();

            foreach (var item in ChannelKey._createdKeys)
            {
                if (item.Key != null)
                    _createdKeys.Add(item.Key, item.Value);
            }

            ChannelKey._createdKeys = _createdKeys;
        }

        /// <summary>
        /// Use this function only if all key in the channeled properties are local.
        /// Make sure that every key has a different ID. 
        /// </summary>
        /// <param name="id">>Unique id</param>
        /// <returns>Created key</returns>
        public static ChannelKey CreateLocalChannelKey(ulong id) => new(id);
        public static ChannelKey GetUniqueChannelKey()
        {
            _internalChannelKeyCount++;
            ChannelKey channelKey = new ChannelKey(_internalChannelKeyCount);

            return channelKey;
        }

        public static ChannelKey GetUniqueChannelKey(object pointer)
        {
            if (_createdKeys.TryGetValue(pointer, out var key))
                return key;

            key = GetUniqueChannelKey();

#if UNITY_EDITOR
            if(pointer is Object unityObject)
                key.pointer = unityObject;

            key.ObjectType = pointer.GetType().Name;
#endif

            //Debug.Log($"Created key with id {key._id} for {pointer.name}", key.pointer);
            _createdKeys.Add(pointer, key);

            return key;
        }

        public static implicit operator ChannelKey(Object unityObject)
        {
            if (_createdKeys.TryGetValue(unityObject, out ChannelKey channelKey))
                return channelKey;
            else
                return GetUniqueChannelKey(unityObject);
        }
        #endregion


        [SerializeField]
        internal ulong _id;

        public ulong ID => _id;
#if UNITY_EDITOR
        [SerializeField]
        internal Object pointer;
        [SerializeField]
        internal string ObjectType;
#endif

        private int hashcode;


        private ChannelKey(ulong id)
        {
            _id = id;
            hashcode = id.GetHashCode();

#if UNITY_EDITOR
            pointer = null;
            ObjectType = null;
#endif
        }

        public override int GetHashCode() => hashcode;

        public override bool Equals(object obj) => base.Equals(obj);


    }
}
