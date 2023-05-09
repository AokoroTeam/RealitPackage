using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.InputSystem;
using NaughtyAttributes;
using LTX;

namespace LTX.ControlsDisplay
{
    public class ControlDisplayer : MonoBehaviour
    {
        [SerializeField]
        List<string> controlSchemes;

        [SerializeField, Required]
        GameObject CommandLayout;

        [SerializeField, Required]
        Transform root;
        public Settings actionSettings;

        [SerializeField, ReadOnly]
        private List<CommandDisplayer> displays = new List<CommandDisplayer>();

        [SerializeField, RequireInterface(typeof(IInputActionsProvider))]
        private UnityEngine.Object _actionProviderReference;

        private IInputActionsProvider _actionProvider;
        private IInputActionsProvider ActionProvider
        {
            get
            {
                if (_actionProvider == null && _actionProviderReference != null)
                    _actionProvider = _actionProviderReference as IInputActionsProvider;

                return _actionProvider;
            }
            set => _actionProvider = value;
        }


        private CanvasGroup canvasGroup;

        private void Awake()
        {
            //Ensure that on creation, everything is ready
            Clean();

            canvasGroup = GetComponent<CanvasGroup>();
        }

        private void OnDestroy()
        {
            if (ActionProvider != null)
                ActionProvider.OnActionsNeedRefresh -= Refresh;
        }


        public void AssignActionProvider(IInputActionsProvider value, bool triggerRefresh = true)
        {
            if (ActionProvider != value)
            {
                if (ActionProvider != null)
                    ActionProvider.OnActionsNeedRefresh -= Refresh;
                
                ActionProvider = value;
                if (value != null)
                {
                    ActionProvider.OnActionsNeedRefresh += Refresh;

                    if (triggerRefresh)
                        Refresh();
                }
                else
                    Clean();
            }
        }

        public void Show()
        {
            if (canvasGroup != null)
                canvasGroup.alpha = 1;
            else
                root.gameObject.SetActive(true);
            Refresh();
        }

        public void Hide()
        {
            if (canvasGroup != null)
                canvasGroup.alpha = 0;
            else
                root.gameObject.SetActive(false);
        }

        [Button("Refresh", EButtonEnableMode.Editor)]
        public void Refresh()
        {
            Clean();

            InputAction[] inputActions = ActionProvider.GetInputActions();
            InputDevice[] devices = ActionProvider.GetDevices();
            string schemeName = ActionProvider.GetControlScheme();

            if (controlSchemes.Count == 0 || controlSchemes.Contains(schemeName))
            {
                if (ControlsDiplaySystem.GetControlsForControlScheme(schemeName, out ControlScheme scheme))
                {

                    Action[] actions = ControlsDiplaySystem.SelectInputActions(inputActions, actionSettings);
                    Command[] commands = ControlsDiplaySystem.ExtractCommands(actions, ref scheme, devices, actionSettings);

                    for (int i = 0; i < commands.Length; i++)
                    {
                        CommandDisplayer displayer = GameObject.Instantiate(CommandLayout, root).GetComponent<CommandDisplayer>();
                        displays.Add(displayer);
                        displayer.Fill(commands[i]);
                    }
                    Canvas.ForceUpdateCanvases();
                    return;
                }
            }

            Debug.LogWarning($"[Command Display] No compatible scheme for {schemeName}");
            Hide();
            //Debug.Log("[Control Display] Controls have been successfuly updated", gameObject);
        }

        private void Clean()
        {
            if (displays == null)
                displays = new List<CommandDisplayer>();
            else
            {
                foreach (var display in displays)
                    Destroy(display.gameObject);

                displays.Clear();
            }
        }
    }
}
