using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LTX.ControlsVisualizer.Abstraction
{
    public struct Input
    {
        public string Path => _path;
        public string DeviceLayout => _deviceLayout;
        public string AdditionnalData => _additionnalData;

        [SerializeField]
        private string _path;
        [SerializeField]
        internal string _deviceLayout;
        [SerializeField]
        public string _additionnalData;


        public Input(string path, string additionnalData, string deviceLayout)
        {
            _path = path;
            _additionnalData = additionnalData;
            _deviceLayout = deviceLayout;

        }
    }
}
