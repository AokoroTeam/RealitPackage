using System.Collections;
using System.Collections.Generic;
using Realit.Core;
using Realit.Core.Scenes;
using UnityEngine;


namespace Realit.Core
{

    [CreateAssetMenu(fileName = "New build content", menuName = "Aokoro/Realit/Reader/Build/BuildContent")]
    public class RealitReaderBuildContent : ScriptableObject
    {
        public RealitBuiltInSceneProfile[] scenes;
    }
}