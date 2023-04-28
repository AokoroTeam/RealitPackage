using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Realit.Core.Scenes
{
    [CreateAssetMenu(fileName = "New scene profile", menuName = "Aokoro/Realit/Reader/Build/BuiltInSceneProfile")]
    public class RealitBuiltInSceneProfile : ScriptableObject
    {
        public string SceneName;
        public Features.FeatureDataAsset[] features;
        public Texture2D ScenePreview;
    }
}
