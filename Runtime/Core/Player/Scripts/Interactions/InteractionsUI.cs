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
            
            GetComponent<Canvas>().worldCamera = Camera.main;
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
                            Hold(holdTime);
                            break;

                        case InputActionPhase.Canceled:
                            StopHold();
                            break;

                        case InputActionPhase.Performed:
                            SucessHold();
                            break;
                    }
                }
            }
        }

        public void Hold(float holdTime)
        {
            if (uiItem != null)
                uiItem.Hold(holdTime);
        }

        public void StopHold()
        {
            if (uiItem != null)
                uiItem.StopHold();
        }

        public void SucessHold()
        {
            if (uiItem != null)
            {
                uiItem.StopHold();
                uiItem.Sucess();
            }
        }

        protected override void OnExistingInstanceFound(InteractionsUI existingInstance)
        {
            Destroy(gameObject);
        }

    }
}
