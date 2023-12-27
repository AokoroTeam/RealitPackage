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
        [SerializeField]
        private Animator uiAnimator;
        [BoxGroup("Painting")]
        [SerializeField]
        private CanvasGroup uiCanvasGroup;

        private bool isActive = false;

        
        private void Start()
        {
            Close();
        }

        protected override void Update()
        {
            base.Update();

            //If too far and active
            if(isActive && !IsPlayerInRange)
            {
                Close();
            }
        }

        public override void Interact()
        {
            if(!isActive)
            {
                Open();
            }
            else
            {
                Close();
            }
        }

        public void Close()
        {
            isActive = false;
            uiAnimator.SetTrigger(closeHashTrigger);
            uiCanvasGroup.interactable = false;
        }

        public void Open()
        {
            isActive = true;
            uiAnimator.SetTrigger(openHashTrigger);
            uiCanvasGroup.interactable = true;
        }

    }
}
