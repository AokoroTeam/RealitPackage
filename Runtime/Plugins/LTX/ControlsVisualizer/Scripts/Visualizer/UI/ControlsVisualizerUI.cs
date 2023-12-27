using LTX.ControlsVisualizer.Abstraction;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LTX.ControlsVisualizer.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class ControlsVisualizerUI : MonoBehaviour
    {
        [SerializeField]
        private ControlUILibrary data;
        [SerializeField]
        private ControlProvider controlProvider;
        [SerializeField]
        private Transform controlParent;

        protected CanvasGroup canvasGroup;

        Dictionary<Control, ControlUI> uis;

        protected virtual void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            
        }

        public void BindToControlProvider(ControlProvider controlProvider)
        {
            if (controlProvider == null) 
                return;

            if (this.controlProvider == null)
                this.controlProvider.onControlsChange -= OnProviderControlChanges;

            this.controlProvider = controlProvider;
            this.controlProvider.onControlsChange += OnProviderControlChanges;

        }

        private void OnProviderControlChanges(ControlsFactory controlFactory)
        {
            ClearUI();
            FillUI(controlFactory, controlProvider.CurrentLayouts);
        }

        private void FillUI(ControlsFactory controlFactory, string[] layouts)
        {

            if (TryGetMatchingLibrary(layouts, out DeviceUILibrary[] librairies))
            {
                foreach (var control in controlFactory.controls)
                {
                    ControlUIContent controlUIContent = new ControlUIContent(control);
                    controlUIContent.CollectCommandsContent(data.commandUI, librairies);

                    var controlUI = Instantiate(data.controlUI, controlParent).GetComponent<ControlUI>();
                    controlUI.FillWithControls(controlUIContent);
                }

                //ControlUIContent controlUIContent = new ControlUIContent(control);
            }
        }


        public bool TryGetMatchingLibrary(string[] layouts, out DeviceUILibrary[] deviceLibrary)
        {
            List<DeviceUILibrary> deviceLibrariesList = new();

            var librairies = data.libraries;

            for (int i = 0; i < librairies.Length; i++)
            {
                DeviceUILibrary lib = librairies[i];
                for (int j = 0; j < layouts.Length; j++)
                {
                    if (lib.MatchesLayout(layouts[j]))
                        deviceLibrariesList.Add(lib);
                }
            }

            deviceLibrary = deviceLibrariesList.ToArray();
            return deviceLibrariesList.Count > 0;
        }

        private void ClearUI()
        {
            foreach (var (_, ui) in uis)
                Destroy(ui);

            uis.Clear();
        }
    }
}
