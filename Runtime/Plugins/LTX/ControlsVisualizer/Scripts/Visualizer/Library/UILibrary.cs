using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using LTX.ControlsVisualizer.Abstraction;
using Input = LTX.ControlsVisualizer.Abstraction.Input;

namespace LTX.ControlsVisualizer
{
    [CreateAssetMenu(menuName = "LTX/Input Visualizer/Library")]
    public class UILibrary : ScriptableObject
    {
        [SerializeField]
        DeviceUILibrary[] libraries;

        internal bool TryGetVisualForInput(Input input, out InputUIData inputVisualData)
        {
            for (int i = 0; i < libraries.Length; i++)
            {
                DeviceUILibrary lib = libraries[i];
                if (lib.MatchesDevice(input.DeviceLayout))
                    return lib.HasMatchingVisual(input, out inputVisualData);
            }

            inputVisualData = default;
            return false;
        }
    }
}
