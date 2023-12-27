using LTX.ChanneledProperties;
using NaughtyAttributes;
using Realit.Core.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Realit.Core.Player.Interactions
{
    public abstract class InteractableObject : Outline
    {
        [SerializeField, BoxGroup("Interactable")]
        private float range = 2;
        [SerializeField, BoxGroup("Interactable")]
        private PriorityTags priority = PriorityTags.Default;
        [SerializeField, BoxGroup("Interactable")]
        private Transform uiPos;

        public Transform UiPos => uiPos;

        public bool IsPlayerInRange { get; internal set; }
        public bool IsVisibleByCamera { get; internal set; }

        public PrioritisedProperty<bool> canInteract;

        private new SphereCollider collider;

        
        protected override void OnEnable()
        {
            base.OnEnable();
            PlayerInteractions.InteractableObjects.Add(this);
        }


        protected override void OnDisable()
        {
            base.OnDisable();
            PlayerInteractions.InteractableObjects.Remove(this);
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            if(collider == null && !TryGetComponent(out collider))
                collider = gameObject.AddComponent<SphereCollider>();

            collider.isTrigger = true;
            collider.radius = range;
        }

        protected override void Awake()
        {
            base.Awake();
            OnValidate();
            canInteract = new PrioritisedProperty<bool>(false);
        }

        public abstract void Interact();
        

        public virtual void SetStateAsTargetInteractable()
        {
            OutlineWidth = 5;
        }

        public virtual void SetStateAsIdle()
        {
            OutlineWidth = 0;
        }

        private void OnTriggerEnter(Collider other)
        {
            //This is the player
            if (other.CompareTag(PlayerInteractions.PlayerTag))
            {
                IsPlayerInRange = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(PlayerInteractions.PlayerTag))
            {
                IsPlayerInRange = false;
            }
        }
    }
}
