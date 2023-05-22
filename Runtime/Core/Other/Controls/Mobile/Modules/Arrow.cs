using Realit.Core.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
namespace Realit.Core.Controls.Modules
{
    public class Arrow : MobileBaseLookSurface
    {
        [SerializeField]
        RectTransform dot;
        [SerializeField]
        RectTransform round;
        [SerializeField]
        RectTransform arrow;
        [SerializeField]
        CanvasGroup canvasGroup;
        [SerializeField]
        float maxSize = 500;
        [SerializeField]
        float minSize = 30;


        Dictionary<MonoBehaviour, Action<Vector2>> observers = new();

        private void Start()
        {
            round.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, minSize * 2);
            round.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, minSize * 2);
        }

        public void AddObserver(MonoBehaviour mono, Action<Vector2> callback)
        {
            observers.TryAdd(mono, callback);
        }

        public void RemoveObserver(MonoBehaviour mono)
        {
            observers.Remove(mono);
        }

        protected override void EvaluateValues()
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
                    Vector2 pos = touch.screenPosition;
                    Vector2 startPos = touch.startScreenPosition;

                    //Sometimes in editor, delta can be NaN.
                    if (!float.IsNaN(pos.x) || !float.IsNaN(pos.y))
                    {
                        wasPressedLastFrame = true;
                        Vector2 delta = Vector2.ClampMagnitude(pos - startPos, maxSize);
                        canvasGroup.alpha = 1;
                        bool bigEnough = delta.sqrMagnitude > minSize * minSize;

                        arrow.transform.gameObject.SetActive(bigEnough);
                        round.transform.gameObject.SetActive(!bigEnough);
                        dot.transform.position = startPos;
                        round.transform.position = startPos;
                        
                        if (bigEnough)
                        {
                            currentSpeed = delta * multiplier;

                            arrow.transform.position = startPos;
                            arrow.transform.right = delta;
                            arrow.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, delta.magnitude);

                            return;
                        }
                        else
                        {
                            currentSpeed = Vector2.Lerp(currentSpeed, Vector2.zero, deceleration * Time.deltaTime);
                            return;
                        }

                    }
                }

                //Else, slow down
                wasPressedLastFrame = false;
                currentSpeed = Vector2.Lerp(currentSpeed, Vector2.zero, deceleration * Time.deltaTime);
                canvasGroup.alpha = 0;
            }
        }

        protected override void InternalDisable(Realit_Player player)
        {
            base.InternalDisable(player);
            canvasGroup.alpha = 0;
        }

        protected override void InternalEnable(Realit_Player player)
        {
            base.InternalEnable(player);
            canvasGroup.alpha = 0;
        }

        protected override void InternalPerform()
        {
            base.InternalPerform();

            foreach (var item in observers)
            {
                item.Value?.Invoke(this.currentSpeed);
            }
        }
    }
}
