using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Realit.Core.Features.Fullscreen
{
    [CreateAssetMenu(fileName = "Fullscreen Data", menuName = "Aokoro/Realit/Features/Fullscreen/Data")]
    public class Fullscreen_Data : FeatureData<Fullscreen>
    {
        public Sprite expand, shrink;

        public override Fullscreen GenerateFeatureFromData() => new Fullscreen(this);

        public override bool CanGenerateFeature() => Application.isEditor || Fullscreen.IsFullScreenSupported;
    }
}
