using Michsky.MUIP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Realit.Core.Features.UI
{
    public class FeatureMobileButton : FeatureIndicator
    {
        [HideInInspector]
        public string featureName;
        public Image icon;
        public ButtonManager buttonManager;
        
        public GameObject webgl;
        public GameObject other;


        public void OnClicked()
        {
            FeaturesManager.ToggleFeature(featureName);
        }

        public override void SetIcon(Sprite sprite)
        {
#if UNITY_WEBGL
            icon.sprite = sprite;
#else
            buttonManager.SetIcon(sprite);
#endif
        }

        protected override void BindToFeature(Feature feature)
        {
            featureName = feature.Data.FeatureName;

#if UNITY_WEBGL
            webgl.SetActive(true);
            other.SetActive(false);
#else
            webgl.SetActive(false);
            other.SetActive(true);
#endif
        }
    }
}
