using Realit.Core.Managers;
using Realit.Core.Player.Movement;
using Realit.Core.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;
using Realit.Core.Player.CameraManagement;

namespace Realit.Core.Controls
{
    public class MobileZoom : MonoBehaviour, MobileControls.IMobileControl
    {
        private float lastMultiTouchDistance;
        private CameraManager cameraManager;

        [SerializeField]
        float multiplier = 1;


        private void ZoomCamera(Touch firstTouch, Touch secondTouch)
        {
            //1
            if (firstTouch.phase == TouchPhase.Began || secondTouch.phase == TouchPhase.Began)
                lastMultiTouchDistance = Vector2.Distance(firstTouch.screenPosition, secondTouch.screenPosition);

            //2
            if (firstTouch.phase != TouchPhase.Moved || secondTouch.phase != TouchPhase.Moved)
                return;

            //3
            float newMultiTouchDistance = Vector2.Distance(firstTouch.screenPosition, secondTouch.screenPosition);
            float pixeldelta = (lastMultiTouchDistance - newMultiTouchDistance) * Time.deltaTime * multiplier;
            //4
            
            cameraManager.ZInput.Write(this, pixeldelta);
            //5
            lastMultiTouchDistance = newMultiTouchDistance;
        }

        void MobileControls.IMobileControl.Perform()
        {
            if (cameraManager == null || !cameraManager.ZInput.OwnsChannel(this))
                return;

            var touches = Touch.activeTouches;
            if (touches.Count == 2)
            {
                if (!MobileControls.IsPointerOverUi(touches[0].screenPosition) && !MobileControls.IsPointerOverUi(touches[1].screenPosition))
                {

                    RectTransform rt = transform as RectTransform;
                    if (RectTransformUtility.RectangleContainsScreenPoint(rt, touches[0].screenPosition) && RectTransformUtility.RectangleContainsScreenPoint(rt, touches[1].screenPosition))
                    {
                        ZoomCamera(touches[0], touches[1]);
                        return;
                    }
                }
            }
            else
            {
                lastMultiTouchDistance = 0;
                cameraManager.ZInput.Write(this, 0);
            }
        }

        void MobileControls.IMobileControl.Enable(Realit_Player player)
        {
            if (player.GetLivingComponent(out cameraManager))
                cameraManager.ZInput.AddChannel(this, Aokoro.PriorityTags.Default);
        }

        void MobileControls.IMobileControl.Disable(Realit_Player player)
        {
            if (player.GetLivingComponent(out cameraManager))
                cameraManager.ZInput.RemoveChannel(this);
        }
    }
}
