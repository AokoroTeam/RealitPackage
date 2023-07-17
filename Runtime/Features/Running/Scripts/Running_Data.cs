using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Screen = UnityEngine.Device.Screen;
using Application = UnityEngine.Device.Application;
using SystemInfo = UnityEngine.Device.SystemInfo;

namespace Realit.Core.Features.Running
{
    [CreateAssetMenu(fileName = "Running Data", menuName = "Aokoro/Realit/Features/Running/Data")]
    public class Running_Data : FeatureData<Running>
    {
        public Sprite run, walk;

        public override Running GenerateFeatureFromData() => new Running(this);

        public override bool CanGenerateFeature() => Application.isMobilePlatform;
    }
}
