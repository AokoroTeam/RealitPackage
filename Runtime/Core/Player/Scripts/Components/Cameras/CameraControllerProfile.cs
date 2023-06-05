using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Realit.Core.Player.CameraManagement
{

    [System.Serializable]
    [CreateAssetMenu(fileName = "CameraControllerProfile", menuName = "Aokoro/Realit/Player/Camera controller Profile")]
    public class CameraControllerProfile : ScriptableObject
    {
        [Tooltip("Tag that describes how the camera behaves")]
        public string Tag;
    }
}
