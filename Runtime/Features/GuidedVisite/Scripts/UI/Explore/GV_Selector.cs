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

        RectTransform myRect;

        public void Awake()
        {
            myRect = transform as RectTransform;
        }
        public void SetSelectedButton(GV_PointButton button)
        {
            selectedPointButton = button;
        }


        protected void Update()
        {
            if (selectedPointButton == null)
                return;

            RectTransform targetRect = selectedPointButton.transform as RectTransform;
            Vector2 targetPos = new Vector2(myRect.position.x, targetRect.position.y);
           
            myRect.position = Vector2.Lerp(myRect.position, targetPos, elasticity * Time.deltaTime);
        }
    }
}
