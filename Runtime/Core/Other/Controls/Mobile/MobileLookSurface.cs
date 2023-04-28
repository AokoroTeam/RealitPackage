using NaughtyAttributes;
using Realit.Core.Managers;
using Realit.Core.Player;
using Realit.Core.Player.CameraManagement;
using UnityEngine;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace Realit.Core.Controls
{
    public class MobileLookSurface : MobileBaseLookSurface, MobileControls.IMobileControl
    {        
        [SerializeField]
        MobileJoystick lookJoystick;
        [SerializeField]
        MobileJoystick movingJoystick;


        protected override void InternalEnable(Realit_Player player)
        {
            lookJoystick.OnPointerDownEvent += LookJoystick_OnPointerDown;
            lookJoystick.OnPointerUpEvent += LookJoystick_OnPointerUp;
            base.InternalEnable(player);
        }

        protected override void InternalDisable(Realit_Player player)
        {
            lookJoystick.OnPointerDownEvent -= LookJoystick_OnPointerDown;
            lookJoystick.OnPointerUpEvent -= LookJoystick_OnPointerUp;
            base.InternalDisable(player);
        }


        protected override void InternalPerform()
        {
            base.InternalPerform();
        }

        #region Callbacks
        private void LookJoystick_OnPointerDown()
        {
            if (playerCamController != null)
            {
                //Debug.Log("Lowering prioriry");
                playerCamController.XInput.ChangeChannelPriority(this, Aokoro.PriorityTags.None);
                playerCamController.YInput.ChangeChannelPriority(this, Aokoro.PriorityTags.None);
            }
        }

        private void LookJoystick_OnPointerUp()
        {
            if (playerCamController != null)
            {
                //Debug.Log("Augmenting prioriry");
                playerCamController.XInput.ChangeChannelPriority(this, Aokoro.PriorityTags.High);
                playerCamController.YInput.ChangeChannelPriority(this, Aokoro.PriorityTags.High);
            }
        }

        #endregion

        protected override int GetTouchIndex()
        {
            //Uses the secondary touche if the first one was already used to move
            if (!wasPressedLastFrame)
                return movingJoystick.IsHolding ? 1 : 0;

            return base.GetTouchIndex();
        }
    }
}
