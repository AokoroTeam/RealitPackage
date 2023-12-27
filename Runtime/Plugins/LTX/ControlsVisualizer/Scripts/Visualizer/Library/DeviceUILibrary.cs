using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LTX.ControlsVisualizer.Abstraction;

namespace LTX.ControlsVisualizer.UI
{
    [System.Serializable]
    public struct DeviceUILibrary
    {
        [SerializeField]
        private string deviceLayout;

        
        [SerializeField]
        internal InputUIData[] inputUIs;

        internal bool MatchesLayout(string deviceLayout)
        {
            return deviceLayout == this.deviceLayout;
        }
    }
}
