using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Realit.Core.Features.UI
{
    public class FeatureKeyboardKey : FeatureIndicator
    {
        [SerializeField]
        TextMeshProUGUI textMesh;
        [SerializeField]
        Image icon;

        public override void SetIcon(Sprite sprite)
        {
            icon.sprite = sprite;
        }

        protected override void BindToFeature(Feature feature)
        {
            var inputAction = FeaturesManager.GetInputActionForFeature(feature);

            if (inputAction != null)
            {
#if UNITY_WEBGL
                textMesh.text = inputAction.GetBindingDisplayString(0, InputBinding.DisplayStringOptions.DontUseShortDisplayNames);
#else
                var displayString = inputAction.GetBindingDisplayString(0, out string device, out string controlPath);
                textMesh.text = controlPath;
#endif
            }

        }
    }
}
