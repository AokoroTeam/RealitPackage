using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Aokoro.UI;

namespace Aokoro.UI
{
    [RequireComponent(typeof(Canvas))]
    public class CanvasSafeArea : UIItem
    {
        public RectTransform[] SafeAreaRects;

        private Rect lastSafeArea = Rect.zero;
        
        void Start()
        {
            lastSafeArea = Screen.safeArea;
            ApplySafeArea();
        }

        void ApplySafeArea()
        {
            for (int i = 0; i < SafeAreaRects.Length; i++)
            {
                RectTransform area = SafeAreaRects[i];
                if (area == null)
                    return;

                Rect safeArea = Screen.safeArea;

                Vector2 anchorMin = safeArea.position;
                Vector2 anchorMax = safeArea.position + safeArea.size;
                anchorMin.x /= canvas.pixelRect.width;
                anchorMin.y /= canvas.pixelRect.height;
                anchorMax.x /= canvas.pixelRect.width;
                anchorMax.y /= canvas.pixelRect.height;

                area.anchorMin = anchorMin;
                area.anchorMax = anchorMax;
            }
        }

        protected override void OnUpdate()
        {
            if (lastSafeArea != Screen.safeArea)
            {
                Debug.Log("[Canvas] Changing safe area");
                lastSafeArea = Screen.safeArea;
                ApplySafeArea();
            }
        }
    }
}
