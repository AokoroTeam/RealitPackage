using Michsky.MUIP;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Aokoro.UI
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