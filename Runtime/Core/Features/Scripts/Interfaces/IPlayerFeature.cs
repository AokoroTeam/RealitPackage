using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Realit.Core.Player;

namespace Realit.Core.Features
{

    public interface IPlayerFeatureComponent
    {
        public RealitPlayer Player { get; set; }
    }
}