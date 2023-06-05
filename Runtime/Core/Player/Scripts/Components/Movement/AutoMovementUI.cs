using Realit.Core.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Realit.Core.Player.Movement
{
    public class AutoMovementUI : MonoBehaviour
    {
        public void Skip()
        {
            if(RealitSceneManager.Player.GetLivingComponent(out PlayerCharacter pc))
                pc.SkipAgentTravel();
        }
    }
}
