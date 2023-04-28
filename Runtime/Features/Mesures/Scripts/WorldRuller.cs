using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using TMPro;
using System;

namespace Realit.Core.Features.Mesures
{
    //[ExecuteInEditMode]
    public class WorldRuller : MonoBehaviour
    {
        [SerializeField, BoxGroup("Visual")]
        SpriteRenderer dots, start, end;
        [SerializeField, BoxGroup("Visual")]
        SpriteRenderer startShadow, endShadow/*, dotsShadow*/;

        [SerializeField, BoxGroup("Visual"), Range(0, 1)]
        float endpointSize;
        [SerializeField, BoxGroup("Visual"), Range(0,1)]
        float dotsSize;
        [SerializeField, BoxGroup("Visual"), Range(0,1)]
        float dotsXMargin;
        
        [SerializeField, BoxGroup("Visual")]
        AnimationCurve alphaForPerpendicularity;
        [SerializeField, BoxGroup("Visual")]
        Color color;

        [SerializeField, BoxGroup("Shadow")]
        Color shadowColor;
        [SerializeField, BoxGroup("Shadow")]
        Vector2 shadowOffset;
        [SerializeField, BoxGroup("Shadow")]
        float shadowSize;

        [SerializeField, BoxGroup("Text")]
        TextMeshPro text;
        [SerializeField, BoxGroup("Text"), Range(0, 3)]
        float YTextOffset;
        [SerializeField, BoxGroup("Text"), Range(0, 10)]
        float textSize;

        [SerializeField, BoxGroup("Wall ref")]
        public Vector3 wallNormal;

        [ShowNativeProperty]
        private Vector3 StartPos => start.transform.position;
        [ShowNativeProperty]
        private Vector3 EndPos => end.transform.position;

        
        [SerializeField, BoxGroup("Animation"), ReadOnly]
        private float alphaMultiplier = 1;
        [SerializeField, BoxGroup("Animation"), Range(.01f, 2)]
        private float  animationDuration = 1;

       
        private void Awake()
        {
            Refresh(true, Camera.main.transform);
            Deactivate();
        }

        private void Update()
        {
            Refresh(false, Camera.main.transform);
        }


        private void Refresh(bool hardRefresh, Transform camera)
        {
            Vector3 center = ((StartPos + EndPos) / 2);
            float realDistance = Vector3.Distance(StartPos, EndPos);
            Vector3 camPosition = camera.position;
            Vector3 ToCamera = (center - camPosition).normalized;

            
            Vector3 right = (EndPos - StartPos).normalized;
            Vector3 forward = (ToCamera - right * Vector3.Dot(ToCamera, right)).normalized;
            Vector3 upward = Vector3.Cross(forward, right);

            Debug.DrawRay(transform.position, upward, Color.green);
            Debug.DrawRay(transform.position, right, Color.red);
            Debug.DrawRay(transform.position, forward, Color.blue);

            float cameraPerpendicularity = MathF.Abs(Vector3.Dot(right, ToCamera));
            Quaternion rotation = Quaternion.LookRotation(forward, upward);

            float alpha = alphaForPerpendicularity.Evaluate(cameraPerpendicularity) * alphaMultiplier;
            Color currentColor = new Color(color.r, color.g, color.b, alpha);

            if (start != null)
            {
                if (hardRefresh)
                {
                    start.transform.localScale = Vector3.one * endpointSize;
                }

                start.transform.forward = (StartPos - camPosition).normalized;
                start.color = currentColor;
            }
            if (end != null)
            {
                if (hardRefresh)
                {
                    end.transform.localScale = Vector3.one * endpointSize;
                }

                end.transform.forward = (EndPos - camPosition).normalized; ;
                end.color = currentColor;
            }

            Vector3 textOffset = GetTextOffset(center, Vector3.Cross(wallNormal, right));

            if (text != null)
            {
                if (hardRefresh)
                {
                    text.transform.position = center + YTextOffset * textOffset;
                    text.transform.forward = -wallNormal;
                    decimal distance = decimal.Round((decimal)realDistance, 1);

                    if (distance < 1)
                        text.text = $"{distance * 100}cm";
                    else
                        text.text = $"{distance}m";
                }

                text.color = currentColor;
            }

            if (dots != null)
            {
                if (hardRefresh)
                {
                    dots.transform.position = center;
                    dots.size = new Vector2(realDistance, dotsSize);
                }
                dots.transform.rotation = rotation;
                dots.color = currentColor;
            }
            
            Color shadowColor = new Color(this.shadowColor.r, this.shadowColor.g, this.shadowColor.b, this.shadowColor.a * alpha);
            if (startShadow != null)
            {
                if (hardRefresh)
                {
                    startShadow.transform.localPosition = shadowOffset;
                    startShadow.transform.localScale = Vector3.one * shadowSize;
                }

                startShadow.color = shadowColor;
            }
            if (endShadow != null)
            {
                if (hardRefresh)
                {
                    endShadow.transform.localPosition = shadowOffset;
                    endShadow.transform.localScale = Vector3.one * shadowSize;
                }
                endShadow.color = shadowColor;
            }
        }

        public void ChangeMesure(Vector3 start, Vector3 end)
        {
            this.start.transform.position = start;
            this.end.transform.position = end;

            Refresh(true, Camera.main.transform);
        }

        private Vector3 GetTextOffset(Vector3 center, Vector3 initialOffset)
        {
            float distance = initialOffset.magnitude;

            if (initialOffset.x < 0)
            {
                if (!Physics.Raycast(center, new Vector3(-initialOffset.x, initialOffset.y, initialOffset.z), distance))
                    initialOffset.x *= -1;
            }
            if(initialOffset.y < 0)
            {
                if (!Physics.Raycast(center, new Vector3(initialOffset.x, -initialOffset.y, initialOffset.z), distance))
                    initialOffset.y *= -1;
            }
            if (initialOffset.z < 0)
            {
                if (!Physics.Raycast(center, new Vector3(initialOffset.x, initialOffset.y, -initialOffset.z), distance))
                    initialOffset.z *= -1;
            }

            return initialOffset;
        }

        internal void Activate()
        {
            gameObject.SetActive(true);

            StopAllCoroutines();
            StartCoroutine(nameof(IFade), true);
        }

        internal void Deactivate()
        {
            StopAllCoroutines();
            StartCoroutine(nameof(IFade), false);
        }

        private IEnumerator IFade(bool activate)
        {
            float targetAlpha = activate ? 1f : 0f;
            float startAlpha = alphaMultiplier;
            float t = 0;

            while (t <= animationDuration)
            {
                t += Time.deltaTime;
                float p = Mathf.InverseLerp(0, animationDuration, t);
                alphaMultiplier = Mathf.Lerp(startAlpha, targetAlpha, p);
                yield return null;
            }

            alphaMultiplier = targetAlpha;
            if (!activate)
                gameObject.SetActive(false);

        }
    }
}
