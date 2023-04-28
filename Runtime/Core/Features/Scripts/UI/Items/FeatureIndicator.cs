using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

namespace Realit.Core.Features.UI
{
    public abstract class FeatureIndicator : MonoBehaviour
    {
        protected Feature feature;

        public void SetFeature(Feature feature)
        {
            this.feature = feature;

            SetIcon(feature.Data.defaultIcon);
            BindToFeature(feature);
        }

        public abstract void SetIcon(Sprite sprite);
        protected abstract void BindToFeature(Feature feature);
    }
}
