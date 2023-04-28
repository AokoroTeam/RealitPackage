using UnityEngine;
using System;
using System.IO;
using System.Linq;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.InputSystem.Utilities;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Aokoro.UI.ControlsDiplaySystem
{
    public static class ControlsDiplaySystem
    {

        private static Data _data;
        internal static Data Data
        {
            get
            {
                if (_data == null)
                {
                    _data = Resources.Load<Data>("ControlDisplay/Data");
#if UNITY_EDITOR
                    if (_data == null)
                    {
                        _data = ScriptableObject.CreateInstance<Data>();
                        AssetDatabase.CreateAsset(_data, Path.Combine("Resources/ControlDisplay/Data"));
                    }
#endif
                }

                return _data;
            }
        }

        public static bool GetControlsForControlScheme(string controlScheme, out ControlScheme scheme) => Data.TryGetControlsForScheme(controlScheme, out scheme);


        public static Command[] ExtractCommands(Action[] actions, ref ControlScheme controlScheme, InputDevice[] devices, Settings settings)
        {
            int length = actions.Length;
            Command[] commands = new Command[actions.Length];

            for (int i = 0; i < length; i++)
                commands[i] = ExtractCommand(actions[i], ref controlScheme, devices);

            return commands;
        }

        public static Command ExtractCommand(Action cd_action, ref ControlScheme controlScheme, InputDevice[] devices)
        {
            int skipBindingCount = 0;
            
            //Action to work with
            InputAction action = cd_action.action;
            //Create the command that will represent the action and the controls associated to it
            Command command = new Command(cd_action.settings.outputName);

            //Going through all bindings inside of the action
            var bindings = action.bindings;
            int bindingCount = bindings.Count;

            for (int j = 0; j < bindingCount; j++)
            {
                if (!cd_action.settings.IsBindingRequested(command.CombinationsCount + 1 + skipBindingCount))
                {
                    skipBindingCount++;
                    continue;
                }

                InputBinding binding = bindings[j];
                //Only the final path is intresting
                string effectivePath = binding.effectivePath;

                //If so, skip because it is already used later on
                if (binding.isPartOfComposite)
                    continue;

                //If the binding is itself a combination of multiple bindings
                if (binding.isComposite)
                {
                    Control composite = ExtractCompositeControls(devices, bindings, j);

                    //Modifiers
                    if (composite.compositeType is "OneModifier" or "TwoModifier")
                    {
                        InputRepresentation[] representations = new InputRepresentation[composite.Lenght];
                        int representationLenght = controlScheme.GetInputRepresentationsFromControls(composite.Split(), representations);
                        command.Addcombination(representationLenght, representations);
                    }
                    //Axis etc...
                    else
                    {
                        InputRepresentation representation = controlScheme.GetInputRepresentationFromControl(composite);
                        command.Addcombination(representation);
                    }
                }

                else if (TryGetControlPathsFromBinding(devices, effectivePath, out string controlPath, out string displayName, out InputDevice device))
                {
                    Control control = new Control(controlPath, displayName, device.displayName);
                    InputRepresentation representation = controlScheme.GetInputRepresentationFromControl(control);
                    command.Addcombination(representation);
                }
            }

            return command;
        }

        private static Control ExtractCompositeControls(InputDevice[] devices, ReadOnlyArray<InputBinding> bindings, int index)
        {

            Control composite = new Control(bindings[index].GetNameOfComposite());

            //Composites parts are after the Composite
            while (true)
            {
                index++;
                if (index >= bindings.Count)
                    break;

                var compositeBinding = bindings[index];

                //Out of the composite
                if (!compositeBinding.isPartOfComposite)
                    break;

                if (TryGetControlPathsFromBinding(devices, compositeBinding.effectivePath, out string compositeControlPath, out string compositeDisplayName, out InputDevice device))
                    composite.AddControl(compositeControlPath, compositeDisplayName, device.displayName);
            }
            return composite;
        }

        private static bool TryGetControlPathsFromBinding(InputDevice[] devices, string bindingPath, out string controlPath, out string displayName, out InputDevice associatedDevice)
        {
            displayName = string.Empty;
            controlPath = string.Empty;
            associatedDevice = null;

            foreach (var device in devices)
            {
                var control = InputControlPath.TryFindControl(device, bindingPath);
                if (control != null)
                {
                    displayName = InputControlPath.ToHumanReadableString(control.path,
                        out string deviceLayoutName,
                        out controlPath,
                        InputControlPath.HumanReadableStringOptions.OmitDevice,
                        device);

                    associatedDevice = device;
                    /*Debug.Log($"Display name : {displayName} | ControlPath : {controlPath} | Device Layout : {deviceLayoutName}");
                    Debug.Log($"{control.path} | {control.name} | {control.variants}  | {control.shortDisplayName} ");*/
                    return true;
                }
            }

            return false;
        }

        public static Action[] SelectInputActions(InputAction[] actions, Settings settings)
        {
            if (!settings.HasValue)
                return SelectInputactions(actions);
            else
                return settings.ConvertInputSystemActions(actions);

        }
        public static Action[] SelectInputactions(InputAction[] actions)
        {
            Action[] cD_InputActions = new Action[actions.Length];
            for (int i = 0; i < actions.Length; i++)
            {
                InputAction action = actions[i];
                cD_InputActions[i] = new Action(action.name, action);
            }

            return cD_InputActions;
        }
    }
}
