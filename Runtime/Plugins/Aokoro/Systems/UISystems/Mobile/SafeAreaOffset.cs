using LTX.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aokoro.UI
{
    public class SafeAreaOffset : UIItem
    {
        public event Action<Rect> OnSafeAreaChanges;

        [SerializeField]
        private float testOffset = 0;
        [SerializeField]
        private float testSize = 0;
        [SerializeField]
        private bool isTest;

        [SerializeField]
        private float baseWidth = 600;
        [SerializeField]
        RectTransform[] offset;
        [SerializeField]
        RectTransform[] expand;
        [SerializeField]
        RectTransform.Edge edge;

        private Rect lastSafeArea = Rect.zero;
        private void OnValidate()
        {
            lastSafeArea = Rect.zero;
        }
        protected override void OnUpdate()
        {
            var safeArea = Screen.safeArea;
            if (lastSafeArea == safeArea)
                return;
            
            float pixelOffset = Mathf.Abs(edge switch
            {
                RectTransform.Edge.Right => canvas.pixelRect.xMax - safeArea.xMax,
                RectTransform.Edge.Left => canvas.pixelRect.xMin - safeArea.xMin,
                RectTransform.Edge.Top => canvas.pixelRect.yMax - safeArea.yMax,
                RectTransform.Edge.Bottom => canvas.pixelRect.yMin - safeArea.yMin,
                _ => 0,
            });

            Debug.Log(pixelOffset);

            for (int i = 0; i < this.offset.Length; i++)
            {
                if (isTest)
                    this.offset[i].SetInsetAndSizeFromParentEdge(edge, testOffset, testSize);
                else
                    this.offset[i].SetInsetAndSizeFromParentEdge(edge, pixelOffset, baseWidth);
            }
            for (int i = 0; i < expand.Length; i++)
            {
                if (isTest)
                    expand[i].SetInsetAndSizeFromParentEdge(edge, 0, testSize + testOffset);
                else
                    expand[i].SetInsetAndSizeFromParentEdge(edge, 0, baseWidth + pixelOffset);
            }
            OnSafeAreaChanges?.Invoke(safeArea);
            lastSafeArea = safeArea;
        }
    }
}
