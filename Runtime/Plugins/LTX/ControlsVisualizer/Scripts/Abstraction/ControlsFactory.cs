using LTX.ControlsVisualizer.Abstraction;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace LTX.ControlsVisualizer
{
    [System.Serializable]
    public class ControlsFactory
    {
        [SerializeField]
        internal List<Control> controls = new();

        //Optimisation
        private List<CommandInput> inputBuffer = new();
        private List<Command> commandBuffer = new();


        public ControlsFactory() : this(ControlsFactorySettings.DefaultSettings)
        {

        }
        public ControlsFactory(ControlsFactorySettings factorySettings) 
        {
            controls = new();
        } 

        public bool RemoveControl(Control control) => controls.Remove(control);
        public bool RemoveControl(string controlName)
        {
            int index = controls.FindIndex(ctx => ctx.name == controlName);
            if(index != -1)
            {
                controls.RemoveAt(index);
                return true;
            }
            return false;
        }

        public bool RemoveControl(InputAction inputAction)
        {
            int index = controls.FindIndex(ctx => ctx.name == inputAction.name);
            if (index != -1)
            {
                controls.RemoveAt(index);
                return true;
            }
            return false;
        }

        public bool AddControl(Control control)
        {
            string name = control.name; 
            int index = controls.FindIndex(ctx => ctx.name == control.name);

            if (string.IsNullOrEmpty(name) || index == -1)
                return false;
            
            controls.Add(control);
            return true;
        }

        public bool AddControl(InputAction inputAction, InputDevice[] devices)
        {
            if(ConvertInputActionToControl(inputAction, devices, out Control control))
            {
                controls.Add(control);
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

            Command command;

            for (int j = 0; j < bindingCount; j++)
            {
                InputBinding binding = bindings[j];

                //If so, skip because it is already used later on
                if (binding.isPartOfComposite)
                    continue;

                if (TryCreateCommand(devices, bindings, ref j, out command))
                {
                    commandBuffer.Add(command);
                }
            }

            control = new Control(action.name, commandBuffer.ToArray());
            return inputBuffer.Count > 0;
        }


        StringBuilder stringBuilder = new StringBuilder();
        private bool TryCreateCommand(InputDevice[] devices, ReadOnlyArray<InputBinding> bindings, ref int index, out Command command)
        {
            InputBinding binding = bindings[index];
            if(binding.isComposite)
            {
                inputBuffer.Clear();
                List<string> layouts = new();
                stringBuilder.Clear();

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

                    var deviceIndex = CreateInputFromInputBinding(devices, compositeBinding, out CommandInput input);
                    if(deviceIndex != -1)
                    {
                        inputBuffer.Add(input);
                        string layout = devices[deviceIndex].layout;
                        if(!layouts.Contains(layout))
                            layouts.Add(layout);

                    }
                }
                stringBuilder.AppendJoin(Command.DeviceLayoutsSeparator, layouts);

                var compositeName = binding.GetNameOfComposite();

                CommandType commandType = compositeName switch
                {
                    "OneModifier" or "TwoModifier" => CommandType.And,
                    "1DAxis" or "2DVector" or "3DVector" => CommandType.Axis,
                    _ => CommandType.None,
                };

                if (commandType == CommandType.None)
                    Debug.LogWarning($"[Control Visualizer] Couldn't reconize composite with name {compositeName}");

                command = new Command(stringBuilder.ToString(), commandType, inputBuffer.ToArray());
                return true;
            }
            else
            {
                var deviceIndex = CreateInputFromInputBinding(devices, binding, out CommandInput input);
                if(deviceIndex != -1)
                {
                    command = new Command(devices[deviceIndex].layout, input);
                    return true;
                }
            }

            command = default;
            return false;
        }

        private int CreateInputFromInputBinding(InputDevice[] devices, InputBinding binding, out CommandInput input)
        {
            input = default;
            int deviceIndex = TryGetControlPathsFromBinding(devices, binding.effectivePath, out string controlPath, out string displayName, out string deviceLayout);

            if (deviceIndex != -1)
            {
                StringBuilder customData = new StringBuilder();
                if (!string.IsNullOrEmpty(binding.interactions))
                {
                    customData
                        .Append("Interactions : ")
                        .Append(binding.interactions);
                }

                input = new CommandInput(controlPath, deviceLayout, displayName, customData.ToString());
            }

            return deviceIndex;
        }

        /// <summary>
        /// Extracts data from input in the context of a device
        /// </summary>
        /// <param name="devices">The device where to find the control path</param>
        /// <param name="bindingPath">Path to find</param>
        /// <param name="controlPath">Out control path. Null if not found.</param>
        /// <param name="displayName">Out display name. Null if not found.</param>
        /// <returns>Index of the founded device. -1 if failure.</returns>
        private static int TryGetControlPathsFromBinding(InputDevice[] devices, string bindingPath, out string controlPath, out string displayName, out string deviceLayout)
        {
            displayName = string.Empty;
            controlPath = string.Empty;
            deviceLayout = string.Empty;

            for (int i = 0; i < devices.Length; i++)
            {
                var device = devices[i];
                var control = InputControlPath.TryFindControl(device, bindingPath);
                if (control != null)
                {
                    displayName = InputControlPath.ToHumanReadableString(control.path,
                        out deviceLayout,
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
