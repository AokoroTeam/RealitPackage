using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Realit.Core.Player.CameraManagement;
using NaughtyAttributes;

namespace Realit.Core.Features.CameraSwitch
{
    [CreateAssetMenu(fileName = "CameraSwitch Profile", menuName = "Aokoro/Realit/Features/CameraSwitch/Profile")]

    public class CameraSwitchProfile : ScriptableObject
    {
        [BoxGroup("Data"), AllowNesting]
        public CameraControllerProfile profile;
        [BoxGroup("UI"), AllowNesting]
        public Texture preview;
        [BoxGroup("UI"), AllowNesting]
        public string label;
        [BoxGroup("UI"), TextArea, AllowNesting]
        public string description;
    }
}
