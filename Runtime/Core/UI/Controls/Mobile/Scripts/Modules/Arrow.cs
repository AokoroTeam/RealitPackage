using Realit.Core.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

namespace Realit.Core.Player.Controls.Modules
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
        Vector2 minMaxSize = new(30,500);
        [SerializeField]
        AnimationCurve curve;

        Canvas canvas;

        Dictionary<MonoBehaviour, Action<Vector2>> observers = new();
        
        private void Awake()
        {
            canvas = GetComponentInParent<Canvas>();
        }

        private void Start()
        {
            round.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, minMaxSize.x * 2);
            round.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, minMaxSize.x * 2);
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
                wasStartTouchValid = false;
                currentSpeed = Vector2.Lerp(currentSpeed, Vector2.zero, deceleration * Time.deltaTime);
            }
            else
            {
                Touch touch = touches[touchIndex];
                
                Vector2 pos = RectTransformUtility.PixelAdjustPoint(touch.screenPosition, transform, canvas);
                Vector2 startPos = RectTransformUtility.PixelAdjustPoint(touch.startScreenPosition, transform, canvas);

                //Debug.Log(touch.phase);
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        wasStartTouchValid = !MobileControls.IsPointerOverUi(touch.startScreenPosition);
                        break;
                    case TouchPhase.Moved or TouchPhase.Stationary:
                        if (!float.IsNaN(pos.x) && !float.IsNaN(pos.y) && wasStartTouchValid)
                        {
                            Vector2 delta = Vector2.ClampMagnitude(pos - startPos, minMaxSize.y); canvasGroup.alpha = 1;

                            float magnitude = delta.magnitude;
                            bool bigEnough = magnitude > minMaxSize.x;

                            arrow.transform.gameObject.SetActive(bigEnough);
                            round.transform.gameObject.SetActive(!bigEnough);
                            dot.transform.position = startPos;
                            round.transform.position = startPos;
                            if (bigEnough)
                            {
                                currentSpeed = curve.Evaluate(Mathf.InverseLerp(minMaxSize.x, minMaxSize.y, magnitude)) * multiplier * delta;

                                arrow.transform.position = startPos;
                                arrow.transform.right = delta;

                                //Debug.Log(magnitude);
                                arrow.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, magnitude / canvas.transform.localScale.x);
                                //arrow.sizeDelta = new Vector2(magnitude, arrow.sizeDelta.y);
                                return;
                            }
                            else
                            {
                                currentSpeed = Vector2.Lerp(currentSpeed, Vector2.zero, deceleration * Time.deltaTime);
                                return;
                            }
                        }
                        break;

                    case TouchPhase.Ended or TouchPhase.Canceled:
                        wasStartTouchValid = false;
                        currentSpeed = Vector2.Lerp(currentSpeed, Vector2.zero, deceleration * Time.deltaTime);
                        canvasGroup.alpha = 0;
                        break;
                }
            }
        }

        protected override void InternalDisable(PlayerControls player)
        {
            base.InternalDisable(player);
            canvasGroup.alpha = 0;
        }

        protected override void InternalEnable(PlayerControls player)
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
