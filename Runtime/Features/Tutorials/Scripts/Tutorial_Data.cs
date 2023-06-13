using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Realit.Core.Features;

namespace Realit.Core.Features.Tutorials
{
    public class Tutorial_Data : FeatureData<Tutorial>
    {
        public override Tutorial GenerateFeatureFromData()
        {
            return new Tutorial(this);
        }
    }
}
