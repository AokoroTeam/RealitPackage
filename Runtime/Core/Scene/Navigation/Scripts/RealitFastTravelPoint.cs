using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Realit.Core.Managers;
using Realit.Core.Player.Movement;
using NaughtyAttributes;
using Aokoro;

namespace Realit.Core.Player.Navigation
{
    public class RealitFastTravelPoint : MonoBehaviour
    {
        [SerializeField, BoxGroup("Travel")]
        private RealitFastTravelPoint nextPoint;
        [SerializeField, BoxGroup("Travel")]
        private LayerMask groundLayer;
        [SerializeField, BoxGroup("Travel")]
        Transform destination;
        [SerializeField, BoxGroup("Travel")]
        Transform canvasParent;

        [SerializeField, BoxGroup("Travel")]
        private ChanneledProperty<bool> interactable;
        [SerializeField, BoxGroup("Travel")]
        private ChanneledProperty<float> alpha;

        [SerializeField, BoxGroup("Fade")]
        AnimationCurve alphaForDistance;
        [SerializeField, BoxGroup("Fade")]
        private float minDistance;
        [SerializeField, BoxGroup("Fade")]
        private float maxDistance;
        [SerializeField, BoxGroup("Fade")]
        private float minInteractableDistance;
        [SerializeField, BoxGroup("Fade")]
        private float maxInteractableDistance;

        private CanvasGroup canvasGroup;
        private PlayerCharacter playerCharacter;

        public Vector3 Destination => Physics.Raycast(destination.position, Vector3.down, out RaycastHit ground, 10, groundLayer) ? ground.point : destination.position;

        private void Awake()
        {
            canvasGroup = GetComponentInChildren<CanvasGroup>();
            canvasGroup.interactable = false;

            interactable = new ChanneledProperty<bool>();
            interactable.AddChannel(this, PriorityTags.Smallest);
            alpha = new ChanneledProperty<float>();
            alpha.AddChannel(this, PriorityTags.Smallest);
            RealitSceneManager.OnPlayerIsSetup += OnPlayerIsSetup;
        }

        private void OnPlayerIsSetup(Realit_Player player) => OnEnable();

        private void OnEnable()
        {
            if(RealitSceneManager.Player != null)
            {
                playerCharacter = RealitSceneManager.Player.GetLivingComponent<PlayerCharacter>();
                playerCharacter.OnAgentStartsMoving += PlayerCharacter_OnAgentStartsMoving;
                playerCharacter.OnAgentStopsMoving += PlayerCharacter_OnAgentStopsMoving;
            }
        }

        private void OnDisable()
        {
            if (playerCharacter != null)
            {
                playerCharacter.OnAgentStartsMoving -= PlayerCharacter_OnAgentStartsMoving;
                playerCharacter.OnAgentStopsMoving -= PlayerCharacter_OnAgentStopsMoving;
            }
        }
        private void PlayerCharacter_OnAgentStartsMoving()
        {
            interactable.AddChannel(playerCharacter, PriorityTags.Highest, false);
            alpha.AddChannel(playerCharacter, PriorityTags.Highest, 0);
        }

        private void PlayerCharacter_OnAgentStopsMoving()
        {
            interactable.RemoveChannel(playerCharacter);
            alpha.RemoveChannel(playerCharacter);
        }



        private void Update()
        {
            alpha.Write(this, 0);
            if (RealitSceneManager.Player != null)
            {
                Vector3 to = RealitSceneManager.Player.transform.position - canvasParent.position;
                to.y = 0;

                Quaternion rot = Quaternion.LookRotation(to, Vector3.up);
                canvasParent.rotation = Quaternion.Slerp(canvasParent.rotation, rot, Time.deltaTime * 3f);

                float distance = to.magnitude;
                if (distance >= minDistance && distance <= maxDistance)
                    alpha.Write(this, alphaForDistance.Evaluate(Mathf.InverseLerp(minDistance, maxDistance, distance)));
                
                interactable.Write(this, distance >= minInteractableDistance && distance <= maxInteractableDistance);
            }

            canvasGroup.alpha = alpha;
            canvasGroup.interactable = interactable;
        }

        public void MoveToNextPoint()
        {
            if(playerCharacter != null)
                playerCharacter.StartAgentTravel(nextPoint.Destination);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, minDistance);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, maxDistance);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, minInteractableDistance);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, maxInteractableDistance);

            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(Destination, destination.position);
        }

    }
}
