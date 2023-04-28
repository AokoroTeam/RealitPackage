using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Realit.Core.Features.Settings
{
    public class SettingWindowActions : FeatureComponent<Settings>
    {
        public void CloseWindow()
        {
            FeaturesManager.EndFeature(Feature.Data.FeatureName);
        }

        public void GoToMainMenu()
        {
            Realit.Instance.LoadMainMenu();
        }

        public void ReloadScene()
        {
            Realit.Instance.LoadScene(Realit.Instance.sceneProfile);
        }

        protected override void OnFeatureInitiate() { }
        protected override void OnFeatureEnds() 
        {
        }
        protected override void OnFeatureStarts() 
        { 
        }
    }
}
