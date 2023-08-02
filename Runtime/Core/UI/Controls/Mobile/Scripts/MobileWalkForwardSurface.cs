using LTX.ChanneledProperties;
using NaughtyAttributes;
using Realit.Core.Player;
using Realit.Core.Player.Movement;
using UnityEngine;
using UnityEngine.UI;
using static Realit.Core.Player.Controls.MobileControls;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

namespace Realit.Core.Player.Controls
{
    public class MobileWalkForwardSurface : MonoBehaviour, IMobileControl
    {
        [SerializeField]
        private double timeForHold = 0.25;
        [SerializeField]
        private float minMoveDistance = 0.2f;
        [SerializeField, Range(0,1)]
        private float multiplier = 0.2f;

        [SerializeField, ReadOnly]
        private bool isHolding;
        [SerializeField, ReadOnly]
        private bool waitingForHold;
        [SerializeField]
        private Image holdCircle;

        private Vector2 currentInput;
        PlayerCharacter playerCharacter;

        private void Awake()
        {

            holdCircle.gameObject.SetActive(false);
        }

        void MobileControls.IMobileControl.Perform()
        {
            EvaluateValues();

            if (playerCharacter != null)
            {
                if (isHolding || waitingForHold)
                    SetPriorityToMax();
                else
                    SetPriorityToMin();

                WriteIntoChannels(currentInput);
            }
        }

        void MobileControls.IMobileControl.Enable(PlayerControls player)
        {
            if (playerCharacter != null || player.Manager.GetLivingComponent(out playerCharacter))
                AddChannels();
        }

        void MobileControls.IMobileControl.Disable(PlayerControls player)
        {
            if (playerCharacter != null || player.Manager.GetLivingComponent(out playerCharacter))
                RemoveChannels();
        }
        private void EvaluateValues()
        {
            var touches = Touch.activeTouches;

            //Null handling
            if (playerCharacter == null || touches.Count != 1)
            {
                isHolding = false;
                currentInput = Vector2.zero;
                return;
            }


            Touch touch = touches[0];

            if (isHolding)
            {
                currentInput = Vector2.up * multiplier;
                holdCircle.gameObject.SetActive(false);
                return;
            }

            WaitForHold(touch);
        }

        float currentTime = 0;

        private void WaitForHold(Touch touch)
        {
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    //Debug.Log("Beginning");
                    //Ensure that finger is not on UI when starting the hold phase
                    waitingForHold = !MobileControls.IsPointerOverUi(touch.startScreenPosition);
                    holdCircle.transform.position = touch.startScreenPosition;
                    currentTime = 0;
                    break;
                case TouchPhase.Moved:
                    //If moved before the timer, then cancel the hold phase
                    if (waitingForHold)
                    {
                        float distance = Vector2.SqrMagnitude(touch.startScreenPosition - touch.screenPosition);
                        if (distance >= minMoveDistance * minMoveDistance)
                        {
                            //Debug.Log("Moved while waiting for hold. Aborting.");
                            waitingForHold = false;
                            isHolding = false;
                        }
                    }
                    break;
                case TouchPhase.Ended:
                    //Debug.Log("Touch has ended.");
                    currentTime = 0;
                    waitingForHold = false;
                    isHolding = false;
                    holdCircle.gameObject.SetActive(false);
                    break;
            }
            holdCircle.gameObject.SetActive(waitingForHold && !isHolding);

            if (waitingForHold && !isHolding)
            {
                currentTime += Time.deltaTime;

                //Animation
                float t = currentTime / (float)timeForHold;

                holdCircle.fillAmount = t;
                        
                //Debug.Log($"Start time : {touch.startTime}. \n Time : {touch.time} \n Total time : {currentTime}");
                if (currentTime >= timeForHold)
                {
                    //Debug.Log("Hold validated.");
                    waitingForHold = false;
                    isHolding = true;
                }
            }
        }

        #region Channels Management
        private void AddChannels()
        {
            playerCharacter.movementInput.AddChannel(this, PriorityTags.None);
        }

        private void RemoveChannels()
        {
            playerCharacter.movementInput.RemoveChannel(this);
        }

        private void SetPriorityToMax()
        {
            if(playerCharacter.movementInput.HasChannel(this))
                playerCharacter.movementInput.ChangeChannelPriority(this, PriorityTags.Default);
        }
        private void SetPriorityToMin()
        {
            if (playerCharacter.movementInput.HasChannel(this))
                playerCharacter.movementInput.ChangeChannelPriority(this, PriorityTags.None);
        }
        private void WriteIntoChannels(Vector2 value)
        {
            playerCharacter.movementInput.Write(this, value);
        }

        #endregion
    }
}
