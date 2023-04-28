using Realit.Core.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Realit.Core.Features.GuidedVisite
{
    public class GV_StartupWindow : MonoBehaviour
    {
        private void Awake()
        {
            RealitSceneManager.Player.Freezed.AddChannel(this, Aokoro.PriorityTags.Highest, true);
            FeaturesManager.Instance.canExecuteFeature.AddChannel(this, Aokoro.PriorityTags.Highest, false);
        }

        private void OnDestroy()
        {
            RealitSceneManager.Player.Freezed.RemoveChannel(this);
            FeaturesManager.Instance.canExecuteFeature.RemoveChannel(this);
        }

        public void SelectGuidedVisit()
        {
            if(FeaturesManager.TryGetFeature(out GuidedVisite guidedVisite))
                FeaturesManager.StartFeature(guidedVisite.Data.FeatureName);

            Destroy(gameObject);
        }

        public void SelectDefaultVisite()
        {
            Destroy(gameObject);
        }
    }
}
