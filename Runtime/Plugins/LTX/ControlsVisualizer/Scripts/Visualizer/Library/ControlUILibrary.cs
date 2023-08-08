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
        internal CommandUIData[] commandUI;
        [SerializeField]
        public DeviceUILibrary[] libraries;

        [SerializeField]
        public GameObject controlUI;
    }
}
