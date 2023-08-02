using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LTX.ControlsVisualizer.Abstraction;

namespace LTX.ControlsVisualizer.UI
{
    [System.Serializable]
    internal struct DeviceUILibrary
    {
        [SerializeField]
        private string deviceLayout;

        [SerializeField]
        private InputVisual[] visuals;

        internal bool MatchesDevice(string deviceLayout)
        {
            return deviceLayout == this.deviceLayout;
        }

        internal bool FillUIDataWithDeviceUI(Command command, ref CommandUIData visual)
        {
            visual.inputVisuals.Clear();

            var inputs = command.Inputs;

            for (int i = 0; i < inputs.Length; i++)
            {
                var input = inputs[i];
                for (int j = 0; j < visuals.Length; j++)
                {
                    InputVisual inputVisual = visuals[j];

                    if (inputVisual.Matches(input.Path))
                        visual.inputVisuals.Add(i, inputVisual.prefab);
                }
            }

            return visual.inputVisuals.Count != 0;
        }
    }
}
