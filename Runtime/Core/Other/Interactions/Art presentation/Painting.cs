using NaughtyAttributes;
using Realit.Core.Player.Interactions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Realit.Core
{
    public class Painting : InteractableObject
    {
        #region Static
        private static int openHashTrigger;
        private static int closeHashTrigger;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void CreateAnimatorHashes()
        {
            openHashTrigger = Animator.StringToHash("Open");
            closeHashTrigger = Animator.StringToHash("Close");
        }
        #endregion

        [BoxGroup("Painting")]
        public Animator uiAnimator;

        private bool isActive = false;

        protected override void Update()
        {
            base.Update();

            //If too far and active
            if(isActive && !PlayerInteractions.interactable.HasChannel(this))
            {
                //Deactivate
                Interact();
            }
        }

        public override void Interact()
        {
            isActive = !isActive;

            if(isActive)
            {
                uiAnimator.SetTrigger(openHashTrigger);
            }
            else
            {
                uiAnimator.SetTrigger(closeHashTrigger);
            }
        }
    }
}
