using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Realit.Core.Features.Mesures
{
    [CreateAssetMenu(fileName = "Mesures", menuName = "Aokoro/Realit/Features/Mesures/Data")]
    public class Mesures_Data : FeatureData<Mesures>
    {
        public override Mesures GenerateFeatureFromData()
        {
            return new Mesures(this);
        }

    }
}
