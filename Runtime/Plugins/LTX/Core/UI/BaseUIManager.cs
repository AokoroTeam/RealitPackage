using System;
using UnityEngine;

namespace LTX.UI
{
    [ExecuteInEditMode, DefaultExecutionOrder(-90), RequireComponent(typeof(Canvas))]
    public class BaseUIManager : MonoBehaviour
    {
        public Canvas canvas;
        public Action OnUpdate;

        protected virtual void Awake()
        {
            canvas = GetComponent<Canvas>();
        }

        protected virtual void Update()
        {
            OnUpdate?.Invoke();
        }

        
    }
}