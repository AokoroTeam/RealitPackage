using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Realit.Core.Scenes
{
    [CreateAssetMenu(fileName = "New Scene", menuName = "Aokoro/Realit/Build/Scene")]
    public class RealitScene : ScriptableObject
    {
        public string SceneName;
        public string SceneDisplayName;
        public Features.FeatureDataAsset[] features;
        public Texture2D ScenePreview;
    }
}
