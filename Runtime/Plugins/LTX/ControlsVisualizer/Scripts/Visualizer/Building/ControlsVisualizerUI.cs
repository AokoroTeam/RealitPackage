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
        private ControlUILibrary library;
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
            FillUI(controlFactory);
        }

        private void FillUI(ControlsFactory controlFactory)
        {
            //Buffer
            List<CommandUIData> datas = new();

            foreach (var (_, control) in controlFactory.controls)
            {
                var commands = control.commands;
                datas.Clear();
                for (int i = 0; i < commands.Length; i++)
                {
                    Command command = commands[i];
                    if (library.TryGetVisualForCommand(command, out CommandUIData data))
                        datas.Add(data);
                }
                if (datas.Count > 0)
                {
                    ControlUI controlUI = CreateControlUI(controlParent);

                    uis.Add(control, controlUI);
                    controlUI.Visualizer = this;

                    controlUI.FillWithCommands(datas);
                }
            }
        }

        public CommandUI CreateCommandUI(Transform parent) => Instantiate(library.commandUI, parent).GetComponent<CommandUI>();
        public ControlUI CreateControlUI(Transform parent) => Instantiate(library.commandUI, parent).GetComponent<ControlUI>();
        private void ClearUI()
        {
            foreach (var (_, ui) in uis)
                Destroy(ui);

            uis.Clear();
        }
    }
}
