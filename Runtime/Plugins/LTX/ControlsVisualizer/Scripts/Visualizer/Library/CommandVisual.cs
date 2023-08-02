using LTX.ControlsVisualizer.Abstraction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Input = LTX.ControlsVisualizer.Abstraction.Input;

namespace LTX.ControlsVisualizer.UI
{
    internal struct CommandVisual
    {
        [SerializeField]
        internal CommandType commandType;
        [SerializeField]
        internal InputVisual[] inputVisuals;

        internal bool HasMatchingVisual(Command command, out GameObject visual)
        {
            if (commandType == command.Type)
            {
                Input[] inputs = command.Inputs;
                for (int i = 0; i < inputs.Length; i++)
                {
                    Input input = inputs[i];
                    InputVisual commandUIRepresentation = inputVisuals[i];

                    if (commandUIRepresentation.Matches(input.Path))
                    {
                        visual = commandUIRepresentation.prefab;
                        return true;
                    }
                }
            }

            visual = null;
            return false;
        }
    }
}
