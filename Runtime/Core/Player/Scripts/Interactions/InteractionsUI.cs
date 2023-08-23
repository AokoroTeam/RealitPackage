using LTX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

namespace Realit.Core.Player.Interactions
{
    public class InteractionsUI : Singleton<InteractionsUI>
    {
        [SerializeField]
        private GameObject uiItemPrefab;

        private InteractableObjectUI uiItem;

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

        protected override void Awake()
        {
            base.Awake();
            uiItem = Instantiate(uiItemPrefab, transform).GetComponent<InteractableObjectUI>();
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
            if (uiItem != null)
            {
                uiItem.gameObject.SetActive(true);
                uiItem.SetPosition(interactable.UiPos);
            }
        }

        private void PlayerInteractions_OnNoInteractable(InteractableObject interactable)
        {
            if (uiItem != null)
            {
                uiItem.gameObject.SetActive(false);
            }
        }

        private void PlayerInteractions_OnTryToInteract(InputAction.CallbackContext ctx)
        {
            if (uiItem != null)
            {
                if (ctx.interaction is HoldInteraction hold)
                {
                    switch (ctx.phase)
                    {
                        case InputActionPhase.Started:
                            float holdTime = hold.duration <= 0 ? InputSystem.settings.defaultHoldTime : hold.duration;
                            uiItem.Hold(holdTime);
                            break;

                        case InputActionPhase.Canceled:
                            uiItem.StopHold();
                            break;

                        case InputActionPhase.Performed:
                            uiItem.StopHold();
                            uiItem.Sucess();
                            break;
                    }
                }
            }
        }

        protected override void OnExistingInstanceFound(InteractionsUI existingInstance)
        {
            Destroy(gameObject);
        }

    }
}
