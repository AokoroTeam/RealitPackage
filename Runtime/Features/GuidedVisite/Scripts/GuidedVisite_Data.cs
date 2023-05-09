using NaughtyAttributes;
using Realit.Core.Player.CameraManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Realit.Core.Features.GuidedVisite
{
    [CreateAssetMenu(fileName = "EasyVisite", menuName = "Aokoro/Realit/Features/EasyVisite/Data")]
    public class GuidedVisite_Data : FeatureData<GuidedVisite>
    {
        [BoxGroup("Global")]
        public CameraControllerProfile profile;

        [BoxGroup("UI")]
        public GameObject window;

        public override GuidedVisite GenerateFeatureFromData()
        {
            return new GuidedVisite(this);
        }

    }
}
