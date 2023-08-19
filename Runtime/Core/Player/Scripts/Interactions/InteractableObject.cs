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
        [BoxGroup("Interactable")]
        public float range = 2;
        [BoxGroup("Interactable")]
        public PriorityTags priority = PriorityTags.Default;
        [BoxGroup("Interactable")]
        public Transform UiPos;

        private new SphereCollider collider;


        PlayerInteractions playerInteractions;

        public PlayerInteractions PlayerInteractions
        { 
            get
            {
                if(playerInteractions == null)
                {
                    RealitPlayer.LocalPlayer.GetLivingComponent(out playerInteractions);
                }

                return playerInteractions;
            }
        }
        
        protected override void OnEnable()
        {
            base.OnEnable();
            if (Managers.RealitSceneManager.Player != null)
            {
                PlayerInteractions.interactable?.AddOnValueChangeCallback(OnInteractableChanges);
            }
        }


        protected override void OnDisable()
        {
            if (RealitSceneManager.Player != null)
            {
                PlayerInteractions.interactable?.RemoveOnValueChangeCallback(OnInteractableChanges);
            }
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
            RealitSceneManager.OnPlayerIsSetup += RealitSceneManager_OnPlayerIsSetup;
        }


        private void OnTriggerEnter(Collider other)
        {
            //This is the player
            if(other.CompareTag(PlayerInteractions.tag))
            {
                if(!PlayerInteractions.interactable.HasChannel(this))
                    PlayerInteractions.interactable.AddChannel(this, priority, this);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(playerInteractions.tag))
            {
                if (PlayerInteractions.interactable.HasChannel(this))
                    PlayerInteractions.interactable.RemoveChannel(this);
            }
        }

        private void RealitSceneManager_OnPlayerIsSetup(RealitPlayer player)
        {
            PlayerInteractions.interactable?.AddOnValueChangeCallback(OnInteractableChanges);
        }

        private void OnInteractableChanges(InteractableObject interactableObject)
        {
            if(interactableObject == this)
                OutlineWidth = 5;
            else
                OutlineWidth = 0;
            
        }

        public abstract void Interact();
    }
}
