using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Realit.Core.Managers;
using LTX.ChanneledProperties;

namespace Realit.Core.Features
{
    [System.Serializable]
    public abstract class Feature
    {
        public bool IsActive;
        public FeatureDataAsset Data { get; private set; }
        public T GetData<T>() where T : FeatureDataAsset => Data as T;

        public event Action OnStartCallbacks;
        public event Action OnEndCallbacks;

        private List<GameObject> loadedRessources;

        protected ChannelKey MyChannelKey => FeaturesManager.ChannelKeys[this];

        public Feature(FeatureDataAsset asset)
        {
            Data = asset;
            loadedRessources = new List<GameObject>();
        }

        internal void InternalToggleFeature()
        {
            if (IsActive)
                InternalEndFeature();
            else

                InternalStartFeature();
        }
        //Shortcut
        public void StartFeature() => FeaturesManager.StartFeature(Data.FeatureName);
        
        //Shortcut
        public void EndFeature() => FeaturesManager.EndFeature(Data.FeatureName);

        internal void InternalStartFeature()
        {
            if (FeaturesManager.Instance.canExecuteFeature)
            {
                IsActive = true;
                //Inherited method for custom behaviors
                OnStart();
                OnStartCallbacks?.Invoke();
            }
        }

        internal void InternalEndFeature()
        {
            IsActive = false;
            //Inherited method for custom behaviors
            OnEnd();
            OnEndCallbacks?.Invoke();
        }

        internal void Load()
        {
            if (Data == null)
                return;

            if (loadedRessources.Count > 0)
                UnLoad();

            if (Data.playerObject != null)
                loadedRessources.Add(RealitSceneManager.Player.AddFeatureObject(Data.playerObject));

            int sceneObjectsCount = Data.sceneObjects == null ? 0 : Data.sceneObjects.Length;
            for (int j = 0; j < sceneObjectsCount; j++)
                loadedRessources.Add(GameObject.Instantiate(Data.sceneObjects[j], FeaturesManager.Instance.transform));
            
            //Inherited method for custom behaviors
            OnLoad();
        }

        internal void UnLoad()
        {
            foreach (var r in loadedRessources)
            {
                if(r != null)
                    GameObject.Destroy(r);
            }

            loadedRessources.Clear();

            //Inherited method for custom behaviors
            OnUnload();
        }

        protected abstract void OnLoad();
        protected abstract void OnUnload();
        protected abstract void OnStart();
        protected abstract void OnEnd();

        #region Logs
        public void LogMessage(string message)
        {
            if (Data.canLog)
                Debug.Log($"[{Data.FeatureName}] {message}");
        }

        public void LogWarning(string message)
        {

            if (Data.canLog)
                Debug.LogWarning($"[{Data.FeatureName}] {message}");
        }

        public void LogError(string message)
        {
            if (Data.canLog)
                Debug.LogError($"[{Data.FeatureName}] {message}");
        }
        #endregion
    }
}