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
        CanvasGroup skipButton; 
        
        
        
        private void OnEnable()
        {
            skipButton.interactable = false;
            skipButton.alpha = 0;

            if (RealitSceneManager.Player.GetLivingComponent(out PlayerCharacter playerCharacter))
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
        private void Update()
        {
            //RealitSceneManager.UI.borderOffsets.Write(this, new Vector4(0, -rectTransform.rect.width + rectTransform.anchoredPosition.x));
        }

        private void PlayerCharacter_OnAgentStopsMoving()
        {
            skipButton.interactable = false;
            skipButton.alpha = 0;
        }

        private void PlayerCharacter_OnAgentStartsMoving()
        {
            skipButton.interactable = true;
            skipButton.alpha = 1;
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
