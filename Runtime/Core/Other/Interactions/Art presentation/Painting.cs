using Realit.Core.Player.Interactions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Realit.Core
{
    public class Painting : InteractableObject
    {
        public override void Interact()
        {
            Debug.Log("Interacting");
        }
    }
}
