using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LTX.Sequencing;

namespace Aokoro.AI.Paths
{
    public abstract class AIWaypointActivity : MonoBehaviour
    {
        public abstract Sequencer ActivitySequence(AIAgent agent);
    }
}
