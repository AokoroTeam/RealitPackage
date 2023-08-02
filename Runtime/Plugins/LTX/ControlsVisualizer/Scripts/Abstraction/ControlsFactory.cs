using LTX.ControlsVisualizer.Abstraction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace LTX.ControlsVisualizer
{
    public class ControlsFactory
    {
        internal Dictionary<string, Control> controls;

        //Optimisation
        private List<Command> commandBuffer;


        public ControlsFactory() : this(ControlsFactorySettings.DefaultSettings)
        {

        }
        public ControlsFactory(ControlsFactorySettings factorySettings) 
        {
            controls = new();
        } 

        public bool RemoveControl(string controlName) => controls.Remove(controlName);
        public bool RemoveControl(Control control) => controls.Remove(control.name);
        public bool RemoveControl(InputAction inputAction) => controls.Remove(inputAction.name);

        public bool AddControl(Control control)
        {
            string name = control.name;
            if (string.IsNullOrEmpty(name) || controls.ContainsKey(name))
                return false;
            
            controls.Add(name, control);
            return true;
        }

        public bool AddControl(InputAction inputAction, InputDevice[] devices)
        {
            if(ConvertInputActionToControl(inputAction, devices, out Control control))
            {
                controls.Add(control.name, control);
                return true;
            }

            return false;
        }

        public void Clear()
        {
            controls.Clear();
        }

        /// <summary>
        /// Converting Input action to control
        /// </summary>
        /// <param name="action"></param>
        /// <param name="devices"></param>
        /// <param name="control"></param>
        /// <returns></returns>
        public bool ConvertInputActionToControl(InputAction action, InputDevice[] devices, out Control control)
        {
            commandBuffer.Clear();
            //Going through all bindings inside of the action
            var bindings = action.bindings;
            int bindingCount = bindings.Count;

            Command addCommand;

            for (int j = 0; j < bindingCount; j++)
            {
                InputBinding binding = bindings[j];

                //If so, skip because it is already used later on
                if (binding.isPartOfComposite)
                    continue;

                //If the binding is itself a combination of multiple bindings
                if (binding.isComposite)
                    addCommand = CreateCompositeCommand(devices, bindings, ref j);
                else
                    addCommand = CreateCommand(devices, binding);

                commandBuffer.Add(addCommand);
            }
            control = new Control(action.name, commandBuffer.ToArray());
            return commandBuffer.Count > 0;
        }

        private Command CreateCommand(InputDevice[] devices, InputBinding binding)
        {
            int deviceIndex = TryGetControlPathsFromBinding(devices, binding.effectivePath, out string compositeControlPath, out string compositeDisplayName);

            if (deviceIndex != -1)
                return new Command(devices[deviceIndex].layout, compositeControlPath);
            
            return default;
        }

        private Command CreateCompositeCommand(InputDevice[] devices, ReadOnlyArray<InputBinding> bindings, ref int index)
        {
            List<Command> subCommands = new();

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

                var command = CreateCommand(devices, compositeBinding);

                if(command.IsValid)
                    subCommands.Add(command);
            }

            var compositeName = bindings[index].GetNameOfComposite();

            CommandType commandType = compositeName switch
            {
                "OneModifier" or "TwoModifier" => CommandType.And,
                "1D-Axis" or "2D-Vector" or "3D-Vector" => CommandType.Axis,
                _ => CommandType.None,
            };

            if (commandType == CommandType.None)
                Debug.LogWarning($"[Control Visualizer] Couldn't reconize composite with name {compositeName}");

            return new Command(subCommands.ToArray(), commandType);
        }

        /// <summary>
        /// Extracts data from input in the context of a device
        /// </summary>
        /// <param name="devices">The device where to find the control path</param>
        /// <param name="bindingPath">Path to find</param>
        /// <param name="controlPath">Out control path. Null if not found.</param>
        /// <param name="displayName">Out display name. Null if not found.</param>
        /// <returns>Index of the founded device. -1 if failure.</returns>
        private static int TryGetControlPathsFromBinding(InputDevice[] devices, string bindingPath, out string controlPath, out string displayName)
        {
            displayName = string.Empty;
            controlPath = string.Empty;

            for (int i = 0; i < devices.Length; i++)
            {
                var device = devices[i];
                var control = InputControlPath.TryFindControl(device, bindingPath);
                if (control != null)
                {
                    displayName = InputControlPath.ToHumanReadableString(control.path,
                        out string deviceLayoutName,
                        out controlPath,
                        InputControlPath.HumanReadableStringOptions.OmitDevice,
                        device);

                    /*Debug.Log($"Display name : {displayName} | ControlPath : {controlPath} | Device Layout : {deviceLayoutName}");
                    Debug.Log($"{control.path} | {control.name} | {control.variants}  | {control.shortDisplayName} ");*/
                    return i;
                }
            }

            return -1;
        }
    }
}
