using System.Collections;
using System.Collections.Generic;
using Realit.Core;
using Realit.Core.Scenes;
using Realit.Core.Features;
using UnityEngine;


namespace Realit.Core
{

    [CreateAssetMenu(fileName = "New build content", menuName = "Aokoro/Realit/Build/BuildContent")]
    public class RealitBuildContent : ScriptableObject
    {
        [SerializeField]
        private RealitScene[] scenes;
        [SerializeField]
        private FeatureDataAsset[] defaultFeatures;

        public RealitScene[] Scenes { get => scenes; set => scenes = value; }
        public FeatureDataAsset[] DefaultFeatures { get => defaultFeatures; }
    }
}