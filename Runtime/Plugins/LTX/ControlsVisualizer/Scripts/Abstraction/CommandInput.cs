using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LTX.ControlsVisualizer.Abstraction
{
    [System.Serializable]
    public struct CommandInput
    {
        public string Path => _path;
        public string DeviceLayout => _deviceLayout;
        public string DisplayName => _displayName;
        public string AdditionnalData => _additionnalData;

        [SerializeField]
        private string _path;
        [SerializeField]
        internal string _deviceLayout;
        [SerializeField]
        public string _displayName;
        [SerializeField]
        public string _additionnalData;


        public CommandInput(string path,string deviceLayout, string displayName, string additionnalData = "")
        {
            _path = path;
            _displayName = displayName;
            _deviceLayout = deviceLayout;
            _additionnalData = additionnalData;

        }
    }
}
