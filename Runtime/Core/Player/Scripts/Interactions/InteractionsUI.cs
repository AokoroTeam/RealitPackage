using LTX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Realit.Core.Player.Interactions
{
    public class InteractionsUI : Singleton<InteractionsUI>
    {
        private GameObject uiItemResource;

        private PlayerInteractions playerInteractions;


        private Dictionary<InteractableObject, InteractableObjectUI> interactables;

        public static void Initialize(PlayerInteractions playerInteractions)
        {
            if(!HasInstance)
            {
                var prefab = Resources.Load<GameObject>("InteractionsCanvas");

                Instantiate(prefab);
            }

            playerInteractions.OnNewInteractable += Instance.PlayerInteractions_OnNewInteractable;
            playerInteractions.OnNoInteractable += Instance.PlayerInteractions_OnNoInteractable;
            playerInteractions.OnTryToInteract += Instance.PlayerInteractions_OnTryToInteract;
        }

        public static void OnPlayerDestroyed(PlayerInteractions playerInteractions)
        {
            if (HasInstance)
            {
                playerInteractions.OnNewInteractable -= Instance.PlayerInteractions_OnNewInteractable;
                playerInteractions.OnNoInteractable -= Instance.PlayerInteractions_OnNoInteractable;
                playerInteractions.OnTryToInteract -= Instance.PlayerInteractions_OnTryToInteract;
                Destroy(Instance);
            }
        }

        private void PlayerInteractions_OnNewInteractable(InteractableObject interactable)
        {

        }
        private void PlayerInteractions_OnNoInteractable(InteractableObject interactable)
        {

        }

        private void PlayerInteractions_OnTryToInteract(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
        {

        }

        protected override void OnExistingInstanceFound(InteractionsUI existingInstance)
        {
            Destroy(gameObject);
        }

    }
}
