using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Realit.Core.Features
{

    public abstract class FeatureComponent<T> : MonoBehaviour where T : Feature
    {
        public T Feature { get; private set; }
        private bool bindedToFeature;

        protected virtual void Awake()
        {
            FeaturesManager.AddFeatureLoadedCallbackListener<T>(BindToFeature);

            if (FeaturesManager.AreFeaturesLoaded && FeaturesManager.TryGetFeature<T>(out T f))
                BindToFeature(f);
        }

        private void OnEnable()
        {
            if (Feature != null && !bindedToFeature)
            {
                Feature.OnStartCallbacks += OnFeatureStarts;
                Feature.OnEndCallbacks += OnFeatureEnds;
                bindedToFeature = true;
            }
        }
        
        private void OnDisable()
        {
            if (Feature != null && bindedToFeature)
            {
                Feature.OnStartCallbacks -= OnFeatureStarts;
                Feature.OnEndCallbacks -= OnFeatureEnds;
                bindedToFeature = false;
            }
        }

        private void BindToFeature(Feature feature)
        {
            if (this.Feature != null)
                return;

            if (feature is T t_feature)
                BindToFeature(t_feature);
            
        }

        private void BindToFeature(T t_feature)
        {
            this.Feature = t_feature;

            if (!bindedToFeature)
            {
                t_feature.OnStartCallbacks += OnFeatureStarts;
                t_feature.OnEndCallbacks += OnFeatureEnds;
            }

            bindedToFeature = true;

            OnFeatureInitiate();
        }

        protected abstract void OnFeatureInitiate();
        protected abstract void OnFeatureStarts();
        protected abstract void OnFeatureEnds();
    }
}