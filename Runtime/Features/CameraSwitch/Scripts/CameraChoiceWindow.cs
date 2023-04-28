using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

using UnityEngine;
using Michsky.MUIP;

using Realit.Core.Player.CameraManagement;
using Realit.Core.Player;

using Screen = UnityEngine.Device.Screen;
using Application = UnityEngine.Device.Application;
using SystemInfo = UnityEngine.Device.SystemInfo;

namespace Realit.Core.Features.CameraSwitch.UI
{
    public class CameraChoiceWindow : FeatureComponent<CameraSwitch>
    {
        [SerializeField]
        private ButtonManager validateButton;

        [SerializeField]
        private ToggleGroup toggleGroup;

        [SerializeField]
        private GameObject choicePrefab;

        private Dictionary<Toggle, CameraControllerProfile> choices = new Dictionary<Toggle, CameraControllerProfile>();


        private void Update()
        {
            validateButton.Interactable(toggleGroup.AnyTogglesOn());
        }


        public void SelectCameraType()
        {
            if (FeaturesManager.TryGetFeature(out CameraSwitch cameraSwitch))
            {
                cameraSwitch.SelectCameraType(choices[toggleGroup.GetFirstActiveToggle()]);
                cameraSwitch.EndFeature();
            }
        }

        public void Cancel()
        {
            if (FeaturesManager.TryGetFeature(out CameraSwitch cameraSwitch))
                cameraSwitch.EndFeature();
        }

        protected override void OnFeatureInitiate()
        {
            for (int i = 0; i < toggleGroup.transform.childCount; i++)
            {
                Transform t = toggleGroup.transform.GetChild(i);

                if (t.gameObject.TryGetComponent(out Toggle toggle))
                    toggleGroup.UnregisterToggle(toggle);

                Destroy(t.gameObject);
            }

            if (Realit_Player.LocalPlayer.GetLivingComponent(out CameraManager manager))
            {
                foreach (var kvp in manager.Controllers)
                {
                    bool valid = false;
                    CameraControllerProfile cameraControllerProfile = kvp.Key;
                    
                    foreach(var switchProfile in Feature.associatedProfiles)
                    {
                        //Bug en build?
                        if(switchProfile.profile.name == cameraControllerProfile.name)
                        {
                            var GO = Instantiate(choicePrefab, toggleGroup.transform);
                            var choice = GO.GetComponent<CameraSelection>();
                            Toggle toggle = GO.GetComponent<Toggle>();

                            choice.BindToProfile(switchProfile);
                            choices.Add(toggle, cameraControllerProfile);
                            toggle.group = toggleGroup;
                            toggleGroup.RegisterToggle(toggle);

                            Feature.LogMessage($"Profile {cameraControllerProfile.name} was found and the corresponding toggle was created");

                            valid = true;
                            break;
                        }
                    }
                    if(!valid)
                        Feature.LogWarning($"Couldn't find profile for {cameraControllerProfile.name}");
                }
            }

            toggleGroup.EnsureValidState();
        }

        protected override void OnFeatureStarts()
        {
            foreach(var choice in choices)
            {
                if(choice.Value.name == Feature.currentProfile.name)
                {
                    choice.Key.isOn = true;
                    Debug.Log($"Activating {choice.Value.name}");
                    //toggleGroup.NotifyToggleOn(choice.Key, true);
                }
            }

            toggleGroup.EnsureValidState();
        }

        protected override void OnFeatureEnds()
        {
            toggleGroup.SetAllTogglesOff(false);
        }
    }
}
