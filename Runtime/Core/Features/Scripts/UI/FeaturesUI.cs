using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Realit.Core.Features.UI
{
    public enum Plateform
    {
        Keyboard,
        Gamepad,
        Mobile,
        None
    }

    public abstract class FeaturesUI : MonoBehaviour
    {
        public abstract Plateform _Plateform { get; }

        [SerializeField] GameObject item;

        Dictionary<Feature, FeatureIndicator> uiForFeatures;

        private void Awake()
        {
            uiForFeatures = new();
        }

        public bool TryGetIndicator(Feature feature, out FeatureIndicator go) => uiForFeatures.TryGetValue(feature, out go);

        public virtual FeatureIndicator AddItem(Feature feature)
        {
            if (uiForFeatures.ContainsKey(feature))
                return uiForFeatures[feature];

            FeatureIndicator go = Instantiate(item, transform).GetComponent<FeatureIndicator>();

            go.SetFeature(feature);
            uiForFeatures.Add(feature, go);
            return go;
        }

        public void Clear()
        {
            uiForFeatures.Clear();

            while(transform.childCount > 0)
                Destroy(transform.GetChild(0).gameObject);
        }
    }
}
