using LTX;
using LTX.ChanneledProperties;
using Realit.Core.Features.UI;
using Realit.Core.Managers;

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace Realit.Core.Features
{
    public class FeaturesManager : Singleton<FeaturesManager>
    {
        [SerializeField] private InputActionMap executeFeatures;
        [SerializeField] private GameObject uiPrefab;
        

        private Dictionary<string, Feature> features;
        private Dictionary<Feature, ChannelKey> channelKeys;
        private Dictionary<Type, List<Action<Feature>>> featuresLoadedCallbacks;
        public PrioritisedProperty<bool> canExecuteFeature = new PrioritisedProperty<bool>(true);

        public static bool AreFeaturesLoaded { get; private set; }


        public static FeaturesUIManager UI;
        public static List<Feature> FeaturesList => new(Features.Values);

        private static Dictionary<Type, List<Action<Feature>>> FeaturesLoadedCallbacks 
        { 
            get => Instance.featuresLoadedCallbacks??= new(); 
            set => Instance.featuresLoadedCallbacks = value; 
        }
        public static Dictionary<string, Feature> Features 
        {
            get => Instance.features ??= new(); 
            set => Instance.features = value; 
        }

        public static Dictionary<Feature, ChannelKey> ChannelKeys
        {
            get => Instance.channelKeys ??= new();
            set => Instance.channelKeys = value;
        }

        protected override void OnExistingInstanceFound(FeaturesManager existingInstance)
        {
            if (existingInstance != this)
            {
                Destroy(gameObject);
            }
        }


        protected override void Awake()
        {
            base.Awake();
            canExecuteFeature.AddOnValueChangeCallback(ctx =>
            {
                if (ctx)
                    executeFeatures.Enable();
                else
                    executeFeatures.Disable();
            });
        }
        private void Update()
        {
            if (executeFeatures != null && executeFeatures.enabled && canExecuteFeature)
            {
                ReadOnlyArray<InputAction> actions = executeFeatures.actions;

                for (int i = 0; i < actions.Count; i++)
                {
                    var inputAction = actions[i];
                    if (inputAction.enabled && inputAction.WasPressedThisFrame())
                    {
                        ToggleFeature(inputAction.name);
                        return;
                    }
                }
            }
        }


        public static void LoadFeatures(FeatureDataAsset[] featureDataAsset)
        {
            AreFeaturesLoaded = false;
            Features = new();

            List<FeatureDataAsset> sortedFeaturesData = new(featureDataAsset);
            sortedFeaturesData.Sort((first, second) => first.loadingPriority.CompareTo(second.loadingPriority));
            sortedFeaturesData.Reverse();
            
            //For GC
            Action<Feature>[] callbacksArray = new Action<Feature>[2];
            List<Action<Feature>> callbacks;
            
            foreach (FeatureDataAsset featureData in sortedFeaturesData)
            {
                if (featureData.CanGenerateFeature())
                {
                    Feature feature = featureData.GenerateFeature();
                    Features.Add(feature.Data.FeatureName, feature);
                    ChannelKeys.Add(feature, ChannelKey.GetUniqueChannelKey(feature));

                    feature.Load();

                    Type featureType = feature.GetType();
                    

                    if (FeaturesLoadedCallbacks.TryGetValue(featureType, out callbacks))
                    {
                        int lenght = callbacks.Count;

                        //Augmenting array size if needed
                        if(lenght > callbacksArray.Length)
                            callbacksArray = new Action<Feature>[lenght];

                        //Copy to array so callbacks method can unsubscribe immediatly and modify list.
                        callbacks.CopyTo(callbacksArray);

                        for (int i = 0; i < lenght; i++)
                            callbacksArray[i]?.Invoke(feature);
                    }
                }
            }

            AreFeaturesLoaded = true;

            Instance.CreateInputs();
            Instance.CreateUI();
        }

        public static void UnloadFeatures()
        {
            foreach ((_, var feature) in Features)
            {
                feature.UnLoad();
            }

            Instance.channelKeys.Clear();
            Instance.features.Clear();
            Instance.canExecuteFeature.Clear();
            ClearCallbacks();
        }
        public static void AddFeatureLoadedCallbackListener<T>(Action<Feature> callback) where T : Feature
        {
            Type tType = typeof(T);
            if (!FeaturesLoadedCallbacks.TryGetValue(tType, out List<Action<Feature>> list))
            {
                list = new List<Action<Feature>>();
                FeaturesLoadedCallbacks.Add(tType, list);
            }

            list.Add(callback);
        }
        public static void RemoveFeatureLoadedCallbackListener<T>(Action<Feature> callback) where T : Feature
        {
            Type tType = typeof(T);
            if (!FeaturesLoadedCallbacks.TryGetValue(tType, out List<Action<Feature>> list))
                return;

            list.Remove(callback);
        }

        public static void ClearCallbacks()
        {
            FeaturesLoadedCallbacks.Clear();
        }

        private void CreateInputs()
        {
            if (Features.Count > 0)
            {
                //Cant be more than 10 because there are only 10 avaiable keys
                ///TODO Add 10 more binded to maj + number
                int featuresCount = Mathf.Min(Features.Count, 10);

                //The map that will hold all actions to executes player features
                executeFeatures = new InputActionMap("executeFeatures");
                executeFeatures.Disable();

                int i = 0;

                foreach (var kvp in Features)
                {
                    string featureName = kvp.Key;
                    Feature feature = kvp.Value;
                    FeatureDataAsset data = feature.Data;

                    if (data.hasInputOverride)
                    {
                        InputAction generated = executeFeatures.AddAction(featureName, InputActionType.Button);

                        ReadOnlyArray<InputBinding> bindings = data.inputOverride.bindings;

                        for (int j = 0; j < bindings.Count; j++)
                            generated.AddBinding(bindings[j].path, groups: "Keyboard&Mouse");

                    }
                    else
                    {
                        if (i <= 9)
                        {
                            ///Creates an action to start executing the feature binded to 1, then 2, then 3, etc....
                            int digit = (i == 9 ? 0 : i + 1);
                            InputAction action = executeFeatures.AddAction(featureName, InputActionType.Button);

                            ///TODO : other devices than keyboard should have bindings too
                            action.AddBinding($"<Keyboard>/{digit}", groups: "Keyboard&Mouse");
                            action.AddBinding($"<Keyboard>/numpad{digit}", groups: "Keyboard&Mouse");

                            i++;
                        }
                    }

                }

                executeFeatures.Enable();
            }
        }

        public void CreateUI()
        {
            var windowItem = RealitSceneManager.UI.GetWindow();
            Debug.Log("setup");
            UI = Instantiate(uiPrefab, windowItem.windowObject.transform).GetComponent<FeaturesUIManager>();

            UI.Refresh(true);
        }


        public static bool TryGetFeature<T>(out T t) where T : Feature
        {
            t = null;

            foreach (var kvp in Features)
            {
                if (kvp.Value is T _t)
                {
                    t = _t;
                    return true;
                }
            }

            return false;
        }

        public static void ToggleFeature(string featureName)
        {
            if (Features.TryGetValue(featureName, out Feature feature))
            {
                //feature.LogMessage("Toggling feature");
                feature.InternalToggleFeature();
            }
        }

        public static void StartFeature(string featureName)
        {
            if (!Instance.canExecuteFeature)
                return;

            if (Features.TryGetValue(featureName, out Feature feature))
            {
                feature.InternalStartFeature();
                bool isConcurrantFeature = feature.Data.isConcurrantFeature;

                if(isConcurrantFeature)
                    Instance.canExecuteFeature.AddChannel(ChannelKeys[feature], PriorityTags.Default, true);
                else
                    Instance.canExecuteFeature.AddChannel(ChannelKeys[feature], PriorityTags.High, false);
            }
        }
        public static void EndFeature(string featureName)
        {
            if (Features.TryGetValue(featureName, out Feature feature))
            {
                feature.InternalEndFeature();
                Instance.canExecuteFeature.RemoveChannel(ChannelKeys[feature]);
            }
        }


        public static InputAction GetInputActionForFeature(Feature feature) => GetInputActionForFeature(feature.Data.FeatureName);
        public static InputAction GetInputActionForFeature(string featureName) => Instance.executeFeatures.FindAction(featureName, false);
        
    }
}
