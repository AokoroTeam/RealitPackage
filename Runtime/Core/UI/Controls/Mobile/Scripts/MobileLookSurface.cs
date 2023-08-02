using LTX.ChanneledProperties;
using Realit.Core.Player;
using UnityEngine;

namespace Realit.Core.Player.Controls
{
    public class MobileLookSurface : MobileBaseLookSurface, MobileControls.IMobileControl
    {        
        [SerializeField]
        MobileJoystick lookJoystick;
        [SerializeField]
        MobileJoystick movingJoystick;


        protected override void InternalEnable(PlayerControls player)
        {
            lookJoystick.OnPointerDownEvent += LookJoystick_OnPointerDown;
            lookJoystick.OnPointerUpEvent += LookJoystick_OnPointerUp;
            base.InternalEnable(player);
        }

        protected override void InternalDisable(PlayerControls player)
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
                playerCamController.XInput.ChangeChannelPriority(this, PriorityTags.None);
                playerCamController.YInput.ChangeChannelPriority(this, PriorityTags.None);
            }
        }

        private void LookJoystick_OnPointerUp()
        {
            if (playerCamController != null)
            {
                //Debug.Log("Augmenting prioriry");
                playerCamController.XInput.ChangeChannelPriority(this, PriorityTags.High);
                playerCamController.YInput.ChangeChannelPriority(this, PriorityTags.High);
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
