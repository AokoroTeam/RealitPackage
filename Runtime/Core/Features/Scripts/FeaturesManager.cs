using LTX;
using LTX.ChanneledProperties;
using LTX.ControlsDisplay;
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
        

        private static Dictionary<string, Feature> features;
        private static Dictionary<Type, List<Action<Feature>>> featuresLoadedCallbacks;

        public static bool AreFeaturesLoaded { get; private set; }

        public ChanneledProperty<bool> canExecuteFeature = new ChanneledProperty<bool>(true);

        public static FeaturesUIManager UI;
        public static List<Feature> FeaturesList => new(Features.Values);

        private static Dictionary<Type, List<Action<Feature>>> FeaturesLoadedCallbacks 
        { 
            get => featuresLoadedCallbacks??= new(); 
            set => featuresLoadedCallbacks = value; 
        }
        public static Dictionary<string, Feature> Features 
        {
            get => features ??= new(); 
            set => features = value; 
        }

        protected override void OnExistingInstanceFound(FeaturesManager existingInstance)
        {
            if (existingInstance != this)
                Destroy(gameObject);
        }


        protected override void Awake()
        {
            base.Awake();
            canExecuteFeature.OnValueChanged += ctx =>
            {
                if (ctx)
                    executeFeatures.Enable();
                else
                    executeFeatures.Disable();
            };
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

            FeaturesLoadedCallbacks.Clear();

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

                //Filters to display accordingly
                CD_ActionSettings[] actionSettings = new CD_ActionSettings[featuresCount];
                int i = 0;

                foreach (var kvp in Features)
                {
                    string featureName = kvp.Key;
                    Feature feature = kvp.Value;
                    FeatureDataAsset data = feature.Data;

                    if (data.hasInputOverride)
                    {
                        InputAction from = data.inputOverride;
                        InputAction generated = executeFeatures.AddAction(from.name, from.type);

                        ReadOnlyArray<InputBinding> bindings = from.bindings;

                        for (int j = 0; j < bindings.Count; j++)
                            generated.AddBinding(bindings[j]);

                    }
                    else
                    {
                        ///Creates an action to start executing the feature binded to 1, then 2, then 3, etc....
                        int digit = (i == 9 ? 0 : i + 1);
                        InputAction action = executeFeatures.AddAction(featureName, InputActionType.Button);

                        ///TODO : other devices than keyboard should have bindings too
                        action.AddBinding($"<Keyboard>/{digit}", groups: "Keyboard&Mouse");
                        action.AddBinding($"<Keyboard>/numpad{digit}", groups: "Keyboard&Mouse");

                    }

                    //Indicates to the UI where to find this actions and how to name them
                    actionSettings[i] = new CD_ActionSettings(featureName, featureName, 1);

                    i++;
                }

                executeFeatures.Enable();
            }
        }

        public void CreateUI()
        {
            var windowItem = RealitSceneManager.UI.GetWindow();
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
                feature.LogMessage("Toggling feature");
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
                    Instance.canExecuteFeature.AddChannel(feature, PriorityTags.Default, true);
                else
                    Instance.canExecuteFeature.AddChannel(feature, PriorityTags.High, false);
            }
        }
        public static void EndFeature(string featureName)
        {
            if (Features.TryGetValue(featureName, out Feature feature))
            {
                feature.InternalEndFeature();
                Instance.canExecuteFeature.RemoveChannel(feature);
            }
        }


        public static InputAction GetInputActionForFeature(Feature feature) => GetInputActionForFeature(feature.Data.FeatureName);
        public static InputAction GetInputActionForFeature(string featureName) => Instance.executeFeatures.FindAction(featureName, false);
        
    }
}
