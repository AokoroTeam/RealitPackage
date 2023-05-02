using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

using Michsky.MUIP;
using static Michsky.MUIP.WindowManager;

using LTX.UI;
using LTX.ChanneledProperties;

namespace Aokoro.UI
{
    [ExecuteInEditMode, DefaultExecutionOrder(-90)]
    public class UIManager : BaseUIManager
    {
        public Transform WindowsParent;
        [SerializeField, Dropdown(nameof(WindowsNames))]
        private string defaultWindow;
        public string DefaultWindow => defaultWindow;

        [SerializeField, ReadOnly]
        private WindowManager baseWindowManager;

        private List<string> WindowsNames() => baseWindowManager != null ? 
            baseWindowManager.windows.Select(ctx => ctx.windowName).ToList() :
            new List<string>() { "No WindowManager" };



        public ChanneledProperty<string> windowPriority;


        private void OnValidate()
        {
            baseWindowManager = GetComponent<WindowManager>();
        }
        protected override void Awake()
        {
            windowPriority = new ChanneledProperty<string>();
            windowPriority.AddChannel(this, 1, defaultWindow);
            OnValidate();
            base.Awake();
        }
        private void Start()
        {
            if (!string.IsNullOrWhiteSpace(DefaultWindow))
                OpenWindow(defaultWindow);
        }

        private void OnEnable()
        {
            windowPriority.OnValueChanged += WindowPriority_OnValueChanged;
        }

        private void OnDisable()
        {
            windowPriority.OnValueChanged -= WindowPriority_OnValueChanged;
        }

        private void WindowPriority_OnValueChanged(string windowName)
        {
            OpenWindow(windowName);
        }
        
        private void OpenWindow(string windowName) => baseWindowManager.OpenWindow(windowName);

        public WindowItem CurrentWindow() => baseWindowManager.windows[baseWindowManager.currentWindowIndex];


        public WindowItem CreateWindow(string windowName, GameObject windowObject)
        {
            var instance = Instantiate(windowObject, WindowsParent);
            instance.SetActive(false);

            return AddExistingWindow(windowName, instance);
        }

        public WindowItem AddExistingWindow(string windowName, GameObject windowObject)
        {
            WindowItem window = new WindowItem();

            window.windowName = windowName;
            window.windowObject = windowObject;

            baseWindowManager.windows.Add(window);

            OpenWindow(windowPriority.Value);
            return window;
        }

        public void DestroyWindow(string windowName)
        {
            WindowItem windowItem = GetWindow(windowName);

            if(CurrentWindow() == windowItem)
                baseWindowManager.NextWindow();

            baseWindowManager.windows.Remove(windowItem);

            if(windowItem.buttonObject != null)
                Destroy(windowItem.buttonObject);
            if(windowItem.windowObject != null)
                Destroy(windowItem.windowObject);
        }

        public WindowItem GetWindow() => GetWindow(DefaultWindow);
        public WindowItem GetWindow(string windowName)
        {
            int index = baseWindowManager.windows.FindIndex(ctx => ctx.windowName == windowName);
            return index == -1 ? null : baseWindowManager.windows[index];
        }
    }
}