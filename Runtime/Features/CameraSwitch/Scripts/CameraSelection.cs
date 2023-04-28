using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

using Screen = UnityEngine.Device.Screen;
using Application = UnityEngine.Device.Application;
using SystemInfo = UnityEngine.Device.SystemInfo;

namespace Realit.Core.Features.CameraSwitch.UI
{
    public class CameraSelection : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, ISelectHandler, IDeselectHandler
    {
        private enum ButtonState
        {
            Clicked,
            Selected,
            Hovered,
        }

        [SerializeField, ReadOnly, EnumFlags]
        private ButtonState state;

        [BoxGroup("Components"), SerializeField]
        private TextMeshProUGUI label;
        [BoxGroup("Components"), SerializeField]
        private TextMeshProUGUI description;
        [BoxGroup("Components"), SerializeField]
        private RawImage preview;
        [BoxGroup("Components"), SerializeField]
        public Toggle toggle;

        private Animator animator;

        private const float animationSpeed = 4;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        private readonly int showInfoAnimatorHash = Animator.StringToHash("ShowInfo");

        public void BindToProfile(CameraSwitchProfile profile)
        {
            if (label != null)
                label.text = profile.label;

            if (preview != null)
                preview.texture = profile.preview;

            if(description != null)
                description.text = profile.description;
        }

        private void Update()
        {
            Vector3 scale = Vector3.one;
            if (Application.isMobilePlatform)
            {
                if(toggle.isOn)
                {
                    scale *= 1.05f;
                    animator.SetBool(showInfoAnimatorHash, true);
                }
                else
                {
                    animator.SetBool(showInfoAnimatorHash, false);
                }
            }
            else
            {
                animator.SetBool(showInfoAnimatorHash, false);
                switch (state)
                {
                    case ButtonState.Clicked:
                        scale *= .95f;
                        break;
                    case ButtonState.Selected:
                        scale *= 1.05f;
                        animator.SetBool(showInfoAnimatorHash, true);
                        break;
                    case ButtonState.Hovered:
                        scale *= 1.025f;
                        animator.SetBool(showInfoAnimatorHash, true);
                        break;
                }
            }


            transform.localScale = Vector3.Lerp(transform.localScale, scale, Time.deltaTime * animationSpeed);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            //Debug.Log("Pointer enter");
            state &= ButtonState.Hovered;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            //Debug.Log("Pointer exit");
            state &= ~ButtonState.Hovered;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            //Debug.Log("Pointer down");
            state &= ButtonState.Clicked;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            //Debug.Log("Pointer up");
            state &= ~ButtonState.Clicked;
        }

        public void OnSelect(BaseEventData eventData)
        {
            //Debug.Log("Selected");
            state &= ButtonState.Selected;
        }

        public void OnDeselect(BaseEventData eventData)
        {
            //Debug.Log("Deselected");
            state &= ~ButtonState.Selected;
        }
    }
}
