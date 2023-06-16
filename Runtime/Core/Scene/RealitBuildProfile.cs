using System.Collections;
using System.Collections.Generic;
using Realit.Core;
using Realit.Core.Scenes;
using UnityEngine;


namespace Realit.Editor
{

    [CreateAssetMenu(fileName = "New build profile", menuName = "Aokoro/Realit/Build/BuildProfile")]
    public class RealitBuildProfile : ScriptableObject
    {
        public string buildName;
        [NaughtyAttributes.Expandable]
        public RealitScene[] scenes;
    }
}