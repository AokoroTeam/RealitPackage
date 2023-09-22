using LTX.ChanneledProperties;
using LTX.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Realit.Core.Player.Interactions
{
    public class PlayerInteractions : MonoBehaviour, IEntityComponent<RealitPlayer>, IUpdateEntityComponent
    {
        public RealitPlayer Manager { get; set; }
        public int InitialisationPriority => 0;
        public string ComponentName => nameof(PlayerInteractions);


        public event Action<InteractableObject> OnNewInteractable;
        public event Action<InteractableObject> OnNoInteractable;
        public event Action<InputAction.CallbackContext> OnTryToInteract;

        public PrioritisedProperty<InteractableObject> interactable;
        public PrioritisedProperty<bool> canInteract;

        private InputAction interactAction;



        private static List<InteractableObject> interactableObjects;
        internal static List<InteractableObject> InteractableObjects 
        {
            get => interactableObjects ??= new List<InteractableObject>(); 
        }

        internal static string PlayerTag { get; private set; }
        public void Initiate(RealitPlayer manager)
        {
            //Debug.Log('a');
            InputActionMap inputActionMap = manager.playerInput.actions.FindActionMap("DefaultGameplay");
            interactAction = inputActionMap.FindAction("Interact");

            canInteract.AddOnValueChangeCallback(ctx =>
            {
                if(interactAction != null)
                {
                    if (ctx)
                        interactAction.Enable();
                    else
                        interactAction.Disable();
                }
            });

            canInteract.AddChannel(manager, PriorityTags.Smallest, !manager.Freezed);
            manager.Freezed.AddOnValueChangeCallback(ctx => canInteract.Write(manager, !ctx), false);

            interactable.AddOnValueChangeCallback(ctx =>
            {
                if(ctx == null)
                    OnNoInteractable?.Invoke(ctx);
                else
                    OnNewInteractable?.Invoke(ctx);
            });

            BindToInputAction();
            PlayerTag = gameObject.tag;
        }

        private void Awake()
        {
            InteractionsUI.Initialize(this);
        }

        private void OnDestroy()
        {
            InteractionsUI.OnPlayerDestroyed(this);
        }

        private void OnEnable()
        {
            BindToInputAction();
        }

        private void OnDisable()
        {
            UnbindToInputAction();
        }

        private void BindToInputAction()
        {
            if (interactAction != null)
            {
                interactAction.started += OnTryInteract;
                interactAction.performed += OnTryInteract;
                interactAction.canceled += OnTryInteract;
            }
        }

        private void UnbindToInputAction()
        {
            if (interactAction != null)
            {
                interactAction.started -= OnTryInteract;
                interactAction.performed -= OnTryInteract;
                interactAction.canceled -= OnTryInteract;
            }
        }

        public void OnUpdate()
        {
            foreach (var interactableObject in interactableObjects)
            {

            }
        }

        /// <summary>
        /// Manual interaction
        /// </summary>
        public void Interact()
        {
            if (!interactable.HasMainChannel)
                return;

            interactable.Value.Interact();
        }
        protected void OnTryInteract(InputAction.CallbackContext ctx)
        {
            if (!interactable.HasMainChannel)
                return;

            switch (ctx.phase)
            {
                case InputActionPhase.Performed:
                    interactable.Value.Interact();
                    break;
                default:
                    OnTryToInteract?.Invoke(ctx);
                    break;
            }
        }
    }
}
