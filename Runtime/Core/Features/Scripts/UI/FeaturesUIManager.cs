using Realit.Core.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using Screen = UnityEngine.Device.Screen;
using Application = UnityEngine.Device.Application;
using SystemInfo = UnityEngine.Device.SystemInfo;
using Aokoro.UI;

namespace Realit.Core.Features.UI
{
    public class FeaturesUIManager : MonoBehaviour
    {
        private static PlayerInput PlayerInput => RealitSceneManager.Player.playerInput;
        public Plateform CurrentPlateforme { get; private set; }

        private CanvasSafeArea safeArea;
        private Dictionary<Plateform, FeaturesUI> uis = new Dictionary<Plateform, FeaturesUI>();
        private void Awake()
        {
            safeArea = GetComponentInParent<CanvasSafeArea>();
        }

        private void OnEnable()
        {
            PlayerInput.onControlsChanged += PlayerInput_onControlsChanged;
            safeArea.SafeAreaRects.Add(transform as RectTransform);
        }

        private void OnDisable()
        {
            PlayerInput.onControlsChanged -= PlayerInput_onControlsChanged;
            safeArea.SafeAreaRects.Remove(transform as RectTransform);
        }

        private void PlayerInput_onControlsChanged(PlayerInput inputs)
        {
            Refresh(false);
        }

        public void Refresh(bool hard)
        {
            if (hard)
            {

                List<Feature> features = FeaturesManager.FeaturesList;

                if (features.Count == 0)
                {
                    gameObject.SetActive(false);
                    return;
                }

                features.Sort(new System.Comparison<Feature>(
                (first, second) =>
                {
                    int firstPriority = first.Data.uiPriority;
                    int secondPriority = second.Data.uiPriority;

                    return secondPriority.CompareTo(firstPriority);
                }
            ));
                LookForUIComponents(features);
            }

            CurrentPlateforme = EvaluatePlateform();
            ActivateCorrectUI();
        }

        private void LookForUIComponents(List<Feature> features)
        {
            //Getting all ui componentds
            var uisComponents = GetComponentsInChildren<FeaturesUI>(true);

            for (int i = 0; i < uisComponents.Length; i++)
            {
                FeaturesUI ui = uisComponents[i];
                if (uis.ContainsKey(ui._Plateform))
                    continue;

                uis.Add(ui._Plateform, ui);
                ui.Clear();

                foreach (var feature in features)
                {
                    var item = ui.AddItem(feature);
                    item.transform.SetSiblingIndex(features.IndexOf(feature));
                }
            }

        }

        public FeaturesUI GetCurrentFeaturesUIPlateform() => uis[CurrentPlateforme];

        public bool TryGetUIForFeature(Feature feature, out FeatureIndicator indicator)
        {
            indicator = null;
            if (CurrentPlateforme == Plateform.None)
                return false;

            if(uis.TryGetValue(CurrentPlateforme, out FeaturesUI fui))
                return fui.TryGetIndicator(feature, out indicator);

            return false;
        }

        private void ActivateCorrectUI()
        {
            foreach (var kvp in uis)
                kvp.Value.gameObject.SetActive(kvp.Key == CurrentPlateforme);
        }

        private Plateform EvaluatePlateform()
        {
            string currentControlScheme = PlayerInput.currentControlScheme;

            return currentControlScheme switch
            {
                "Keyboard&Mouse" => Plateform.Keyboard,
                "Gamepad" => Plateform.Gamepad,
                "Mobile" => Plateform.Mobile,
                _ => Plateform.None,
            };
        }
    }
}
