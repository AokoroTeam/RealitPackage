using Realit.Core.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Realit.Core.Player.Interactions
{
    public class InteractableObjectUI : MonoBehaviour
    {
        [SerializeField]
        float mobileHoldTime = .3f;
        [SerializeField]
        private GameObject mobileUI;
        [SerializeField]
        private GameObject desktopUI;
        [SerializeField]
        private Image holdCircle;

        private static PlayerInteractions playerInteractions;

        private static PlayerInteractions PlayerInteractions 
        {
            get
            {
                if(playerInteractions == null && RealitPlayer.LocalPlayer != null)
                {
                    playerInteractions = RealitPlayer.LocalPlayer.GetLivingComponent<PlayerInteractions>();
                }

                return playerInteractions;
            } 
        }

        private void Awake()
        {
            holdCircle.fillAmount = 0;
        }

        internal void SetPosition(Transform uiPos)
        {
            transform.SetPositionAndRotation(uiPos.position, uiPos.rotation);
        }

        public void MobileHold()
        {
            //Debug.Log("bah?");
            StopAllCoroutines();

            holdCircle.enabled = true;
            StartCoroutine(IMobileHold());
        }

        public void MobileStopHold() => StopHold();

        private IEnumerator IMobileHold()
        {
            //Debug.Log("Start Hold");
            yield return IHold(mobileHoldTime);
            StopHold();

            //Debug.Log("End Hold");
            PlayerInteractions.Interact();
        }

        internal void Hold(float maxHoldTime)
        {
            StopAllCoroutines();

            holdCircle.enabled = true;
            StartCoroutine(IHold(maxHoldTime));
        }

        private IEnumerator IHold(float maxHoldTime)
        {
            float startHoldTime = Time.time;
            float holdTime = 0;
            while (holdTime <= maxHoldTime)
            {
                //Debug.Log(holdTime);
                holdTime = Time.time - startHoldTime;
                holdCircle.fillAmount = holdTime / maxHoldTime;
                yield return null;  
            }

            holdCircle.fillAmount = 1;
        }


        internal void StopHold()
        {
            StopAllCoroutines();

            holdCircle.enabled = false;
            holdCircle.fillAmount = 0;
        }

        internal void Sucess()
        {
            StopAllCoroutines();

            holdCircle.enabled = false;
            holdCircle.fillAmount = 0;
        }

        private void OnEnable()
        {
            if(RealitSceneManager.Player != null)
            {
                AdaptVisualToPlayerControls(RealitSceneManager.Player);
            }

            RealitSceneManager.OnPlayerIsSetup += AdaptVisualToPlayerControls;
        }


        private void AdaptVisualToPlayerControls(RealitPlayer player)
        {
            string scheme = player.playerInput.currentControlScheme;

            switch (scheme)
            {
                case "Keyboard&Mouse":
                    mobileUI.SetActive(false);
                    desktopUI.SetActive(true);
                    break;
                case "Mobile":
                    mobileUI.SetActive(true);
                    desktopUI.SetActive(false);
                    break;
                case "Gamepad":
                    desktopUI.SetActive(false);
                    desktopUI.SetActive(false);
                    break;
                default:
                    Debug.Log($"[Interactions UI] Couldn't find ui for scheme {scheme}", this);
                    gameObject.SetActive(false);
                    break;
            }
        }
    }
}
