using Aokoro.UI;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Realit.Core.Features.GuidedVisite.UI
{
    public class GV_Selector : MonoBehaviour
    {
        [SerializeField, ReadOnly]
        GV_PointButton selectedPointButton;
        [SerializeField]
        private float elasticity;

        Image image;
        public void SetSelectedButton(GV_PointButton button)
        {
            selectedPointButton = button;
        }

        private void Awake()
        {
            image = GetComponent<Image>();
        }

        protected void Update()
        {
            if (selectedPointButton == null)
                return;

            RectTransform targetRect = selectedPointButton.transform as RectTransform;
            RectTransform myRect = transform as RectTransform;

            myRect.sizeDelta = Vector2.Lerp(myRect.sizeDelta, targetRect.sizeDelta, elasticity * Time.deltaTime);
            myRect.localPosition = Vector2.Lerp(myRect.localPosition, targetRect.localPosition, elasticity * Time.deltaTime);

            //Changing scale seems to make the image disappear?????
            //myRect.localScale = Vector2.Lerp(myRect.localScale, targetRect.localScale, elasticity * Time.deltaTime);

        }
    }
}
