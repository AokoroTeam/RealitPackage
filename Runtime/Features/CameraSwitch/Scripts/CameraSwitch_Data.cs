using NaughtyAttributes;
using Realit.Core.Player.CameraManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Realit.Core.Features.CameraSwitch
{
    [CreateAssetMenu(fileName = "CameraSwitch", menuName = "Aokoro/Realit/Features/CameraSwitch/Data")]
    public class CameraSwitch_Data : FeatureData<CameraSwitch>
    {
        public override CameraSwitch GenerateFeatureFromData() => new CameraSwitch(this);

    }
}
