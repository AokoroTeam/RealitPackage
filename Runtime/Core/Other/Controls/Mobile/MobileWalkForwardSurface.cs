using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Realit.Core.Player;
using Realit.Core.Player.Movement;

using static Realit.Core.Controls.MobileControls;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;


namespace Realit.Core.Controls
{
    public class MobileWalkForwardSurface : MonoBehaviour, IMobileControl
    {
        [SerializeField]
        private double timeForHold = 0.25;
        [SerializeField]
        private float minMoveDistance = 0.2f;
        [SerializeField, Range(0,1)]
        private float multiplier = 0.2f;

        private bool isHolding;
        private bool waitingForHold;

        private Vector2 currentInput;
        PlayerCharacter playerCharacter;
            
        void MobileControls.IMobileControl.Perform()
        {
            EvaluateValues();

            if (playerCharacter != null)
                WriteIntoChannels(currentInput);

        }

        void MobileControls.IMobileControl.Enable(Realit_Player player)
        {
            if (playerCharacter != null || player.GetLivingComponent(out playerCharacter))
                AddChannels();
        }

        void MobileControls.IMobileControl.Disable(Realit_Player player)
        {
            if (playerCharacter != null || player.GetLivingComponent(out playerCharacter))
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
                return;
            }

            WaitForHold(touch);
        }

        private void WaitForHold(Touch touch)
        {
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    //Debug.Log("Beginning");
                    //Ensure that finger is not on UI when starting the hold phase
                    waitingForHold = !MobileControls.IsPointerOverUi(touch.startScreenPosition);
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
                    waitingForHold = false;
                    isHolding = false;
                    break;
            }

            if (waitingForHold && !isHolding)
            {
                //Debug.Log($"Start time : {touch.startTime}. \n Time : {touch.time} \n Total time : {touch.startTime - touch.time}");
                if (touch.time - touch.startTime >= timeForHold)
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
            playerCharacter.movementInput.AddChannel(this, Aokoro.PriorityTags.Default);
        }

        private void RemoveChannels()
        {
            playerCharacter.movementInput.RemoveChannel(this);
        }

        private void WriteIntoChannels(Vector2 value)
        {
            playerCharacter.movementInput.Write(this, value);
        }

        #endregion
    }
}
