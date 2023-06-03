using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LTX.ControlsVisualizer.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class ControlsUI : MonoBehaviour
    {
        protected CanvasGroup canvasGroup;


        protected virtual void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public virtual void FillWith()
        {

        }
    }
}
