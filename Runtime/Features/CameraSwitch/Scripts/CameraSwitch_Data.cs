using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Realit.Core.Features.CameraSwitch
{
    [CreateAssetMenu(fileName = "CameraSwitch", menuName = "Aokoro/Realit/Features/CameraSwitch/Data")]
    public class CameraSwitch_Data : FeatureData<CameraSwitch>
    {
        [BoxGroup("UI"), SerializeField]
        public string windowName;
        [BoxGroup("UI"), SerializeField]
        public GameObject window;

        [BoxGroup("Global"), SerializeField, Expandable, AllowNesting]
        public CameraSwitchProfile[] profiles;

        public override CameraSwitch GenerateFeatureFromData() => new CameraSwitch(this);

    }
}
