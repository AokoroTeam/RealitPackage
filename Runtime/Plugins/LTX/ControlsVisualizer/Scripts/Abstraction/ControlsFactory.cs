using LTX.ControlsVisualizer.Abstraction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LTX.ControlsVisualizer
{
    public class ControlsFactory
    {
        private List<Control> controls;

        //Optimisation
        private List<Command> commandBuffer;


        public ControlsFactory() : this(ControlsFactorySettings.DefaultSettings)
        {
        }
        public ControlsFactory(ControlsFactorySettings factorySettings) 
        {
            controls = new();
        } 

        //Converting input action to input
        public void AddControl(InputAction inputAction)
        {

        }

        public void AddControl()
        {

        }


        public Control ConvertInputActionToControl(InputAction action)
        {
            int skipBindingCount = 0;

            commandBuffer.Clear();
            /*
            //Going through all bindings inside of the action
            var bindings = action.bindings;
            int bindingCount = bindings.Count;

            for (int j = 0; j < bindingCount; j++)
            {
                if (!cd_action.settings.IsBindingRequested(control.CombinationsCount + 1 + skipBindingCount))
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
                        control.Addcombination(representationLenght, representations);
                    }
                    //Axis etc...
                    else
                    {
                        InputRepresentation representation = controlScheme.GetInputRepresentationFromControl(composite);
                        control.Addcombination(representation);
                    }
                }

                else if (TryGetControlPathsFromBinding(devices, effectivePath, out string controlPath, out string displayName, out InputDevice device))
                {
                    Control control = new Control(controlPath, displayName, device.displayName);
                    InputRepresentation representation = controlScheme.GetInputRepresentationFromControl(control);
                    control.Addcombination(representation);
                }
            }
            return control;
            */
            return default;
        }
    }
}
