using LTX.ChanneledProperties;
using NaughtyAttributes;
using Realit.Core.Player;
using Realit.Core.Player.CameraManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace Realit.Core.Player.Controls
{
    public class MobileBaseLookSurface : MonoBehaviour, MobileControls.IMobileControl
    {
        [SerializeField]
        protected float multiplier = 1;
        [SerializeField]
        protected float deceleration;

        [ShowNonSerializedField]
        protected Vector2 currentSpeed;

        [ShowNonSerializedField]
        protected bool wasPressedLastFrame;
        [ShowNonSerializedField]
        protected int touchIndex = 0;

        protected CameraManager playerCamController;


        void MobileControls.IMobileControl.Perform()
        {
            InternalPerform();
        }


        void MobileControls.IMobileControl.Enable(PlayerControls player)
        {
            InternalEnable(player);
        }

        void MobileControls.IMobileControl.Disable(PlayerControls player)
        {
            InternalDisable(player);
        }

        protected virtual void InternalPerform()
        {

            EvaluateValues();
            if (playerCamController != null)
                WriteIntoChannels(playerCamController);
        }

        protected virtual void InternalEnable(PlayerControls player)
        {
            if (player.Manager.GetLivingComponent(out CameraManager newCam))
            {
                if (playerCamController != null && newCam != playerCamController)
                    RemoveChannels(playerCamController);

                playerCamController = newCam;

                AddChannels(newCam);
            }
        }

        protected virtual void InternalDisable(PlayerControls player)
        {
            if (playerCamController != null)
                RemoveChannels(playerCamController);
        }

        protected virtual void EvaluateValues()
        {

            //Null handling
            if (playerCamController == null)
            {
                currentSpeed = Vector2.zero;
                return;
            }

            var touches = Touch.activeTouches;
            int touchCount = touches.Count;
            int touchIndex = GetTouchIndex();

            //Slowing down if there is no matching touch (either no touch or just one that is used too move)
            if (touchCount != touchIndex + 1)
            {
                wasPressedLastFrame = false;
                currentSpeed = Vector2.Lerp(currentSpeed, Vector2.zero, deceleration * Time.deltaTime);
            }
            else
            {

                Touch touch = touches[touchIndex];

                //Only use hold that is not pointing on anything (to prevent moving when using UI)
                if (!touch.isTap && touch.isInProgress && !MobileControls.IsPointerOverUi(touch.startScreenPosition))
                {
                    Vector2 delta = touch.delta;

                    //Sometimes in editor, delta can be NaN.
                    if (!float.IsNaN(delta.x) || !float.IsNaN(delta.y))
                    {
                        wasPressedLastFrame = true;
                        currentSpeed = delta * multiplier;
                        return;
                    }
                }

                //Else, slow down
                wasPressedLastFrame = false;
                currentSpeed = Vector2.Lerp(currentSpeed, Vector2.zero, deceleration * Time.deltaTime);
            }
        }

        protected virtual void ProcessTouch(Touch touch)
        {
            //Only use hold that is not pointing on anything (to prevent moving when using UI)
            if (!touch.isTap && touch.isInProgress && !MobileControls.IsPointerOverUi(touch.startScreenPosition))
            {
                Vector2 delta = touch.delta;

                //Sometimes in editor, delta can be NaN.
                if (!float.IsNaN(delta.x) || !float.IsNaN(delta.y))
                {
                    wasPressedLastFrame = true;
                    currentSpeed = delta * multiplier;
                    return;
                }
            }

            //Else, slow down
            wasPressedLastFrame = false;
            currentSpeed = Vector2.Lerp(currentSpeed, Vector2.zero, deceleration * Time.deltaTime);
        }

        protected virtual int GetTouchIndex()
        {
            int touchIndex = this.touchIndex;
            return touchIndex;
        }



        #region Channels Management
        private void AddChannels(CameraManager cam)
        {
            cam.XInput.AddChannel(this, PriorityTags.High, 0);
            cam.YInput.AddChannel(this, PriorityTags.High, 0);
        }
        private void RemoveChannels(CameraManager cam)
        {
            cam.XInput.RemoveChannel(this);
            cam.YInput.RemoveChannel(this);
        }

        private void WriteIntoChannels(CameraManager cam)
        {
            cam.XInput.Write(this, currentSpeed.x);
            cam.YInput.Write(this, currentSpeed.y);
        }
        #endregion

    }
}
