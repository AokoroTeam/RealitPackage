using Michsky.MUIP;
using Realit.Core.Managers;
using Realit.Core.Player.Movement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Realit.Core.Features.GuidedVisite.UI
{
    public class GV_Window : MonoBehaviour
    {
        [SerializeField]
        ButtonManager skipButton;

        private void OnEnable()
        {
            if(RealitSceneManager.Player.GetLivingComponent(out PlayerCharacter playerCharacter))
            {
                playerCharacter.OnAgentStartsMoving += PlayerCharacter_OnAgentStartsMoving;
                playerCharacter.OnAgentStopsMoving += PlayerCharacter_OnAgentStopsMoving;
            }
        }

        private void OnDisable()
        {
            if (RealitSceneManager.Player.GetLivingComponent(out PlayerCharacter playerCharacter))
            {
                playerCharacter.OnAgentStartsMoving -= PlayerCharacter_OnAgentStartsMoving;
                playerCharacter.OnAgentStopsMoving -= PlayerCharacter_OnAgentStopsMoving;
            }
        }

        private void PlayerCharacter_OnAgentStopsMoving()
        {
            skipButton.Interactable(false);
        }

        private void PlayerCharacter_OnAgentStartsMoving()
        {
            skipButton.Interactable(true);
        }

        public void Skip()
        {
            if (RealitSceneManager.Player.GetLivingComponent(out PlayerCharacter playerCharacter))
                playerCharacter.SkipAgentTravel();
        }
        public void Exit()
        {
            if (FeaturesManager.TryGetFeature(out GuidedVisite ev))
                ev.EndFeature();
        }
    }
}
