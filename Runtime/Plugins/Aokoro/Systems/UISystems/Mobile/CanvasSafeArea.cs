using LTX.UI;
using UnityEngine;

using Screen = UnityEngine.Device.Screen;
using Application = UnityEngine.Device.Application;
using SystemInfo = UnityEngine.Device.SystemInfo;

namespace Aokoro.UI
{
    [RequireComponent(typeof(Canvas)), ExecuteInEditMode]
    public class CanvasSafeArea : MonoBehaviour
    {
        public RectTransform[] SafeAreaRects;

        private Rect lastSafeArea = Rect.zero;

        private Canvas canvas;
        public Canvas C
        {
            get
            {
                if(canvas == null)
                    canvas = GetComponent<Canvas>();

                return canvas;
            }
        }
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
                anchorMin.x /= C.pixelRect.width;
                anchorMin.y /= C.pixelRect.height;
                anchorMax.x /= C.pixelRect.width;
                anchorMax.y /= C.pixelRect.height;

                area.anchorMin = anchorMin;
                area.anchorMax = anchorMax;
            }
        }

        private void LateUpdate()
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
