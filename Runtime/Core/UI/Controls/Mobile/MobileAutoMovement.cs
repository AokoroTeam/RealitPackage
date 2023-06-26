using LTX.Settings;
using EasyCharacterMovement;
using Realit.Core.Managers;
using Realit.Core.Player;
using Realit.Core.Player.Movement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.EnhancedTouch;

using static Realit.Core.Player.Controls.MobileControls;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace Realit.Core.Player.Controls
{
    public class MobileAutoMovement : MonoBehaviour, IMobileControl
    {
        private const float tapSpeed = .15f;

        private PlayerCharacter playerCharacter;
        

        private bool IsTap(Touch touch)
        {
            bool isMultiTap = touch.isTap && touch.tapCount >= 2;
            if(isMultiTap)
                return touch.history[^1].startTime - touch.startTime <= tapSpeed;
            else
                return false;
        }

        void IMobileControl.Perform()
        {
            if (MainSettingsManager.IsValid && MainSettingsManager.TryGetSettingValue("AutoMovement", out bool enabled) && !enabled)
                return;

            var touches = Touch.activeTouches;

            for (int i = 0; i < touches.Count; i++)
            {
                Touch touch = touches[i];
                if (touch.valid && IsTap(touch))
                {
                    Vector2 screenPosition = touch.screenPosition;

                    if (!IsPointerOverUi(screenPosition))
                        playerCharacter.MoveToScreenLocationAsAgent(screenPosition);

                    return;
                }
            }
        }

        void IMobileControl.Enable(PlayerControls player)
        {
            if (!player.Manager.GetLivingComponent(out playerCharacter))
                gameObject.SetActive(false);
            else
                gameObject.SetActive(true);
        }

        void IMobileControl.Disable(PlayerControls player)
        {

        }
    }
}
