using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using LTX.ControlsVisualizer.Abstraction;

namespace LTX.ControlsVisualizer.UI
{
    [CreateAssetMenu(menuName = "LTX/Control Visualizer/Library")]
    public class ControlUILibrary : ScriptableObject
    {
        [SerializeField]
        DeviceUILibrary[] libraries;
        [SerializeField]
        public GameObject commandUI;
        [SerializeField]
        public GameObject controlUI;

        internal bool TryGetVisualForCommand(Command command, out CommandUIData commandUiData)
        {
            commandUiData = new CommandUIData(command, commandUI);

            for (int i = 0; i < libraries.Length; i++)
            {
                DeviceUILibrary lib = libraries[i];
                if (lib.MatchesDevice(command.DeviceLayout))
                    return lib.FillUIDataWithDeviceUI(command, ref commandUiData);
            }

            return false;
        }
    }
}
