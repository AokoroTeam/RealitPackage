using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LTX.ControlsVisualizer.Abstraction;
using Input = LTX.ControlsVisualizer.Abstraction.Input;


namespace LTX.ControlsVisualizer
{
    [System.Serializable]
    internal struct DeviceUILibrary
    {
        [SerializeField]
        private string deviceLayout;

        [SerializeField]
        private InputUIContent[] visuals;

        internal bool MatchesDevice(string deviceLayout)
        {
            return deviceLayout == this.deviceLayout;
        }

        internal bool HasMatchingVisual(Input input, out InputUIData inputVisualData)
        {
            for (int i = 0; i < visuals.Length; i++)
            {
                if (visuals[i].Matches(input.Path))
                {
                    inputVisualData = visuals[i].GetVisual(input.Path, input.AdditionnalData);
                    return true;
                }
            }

            inputVisualData = default;
            return false;
        }
    }
}
