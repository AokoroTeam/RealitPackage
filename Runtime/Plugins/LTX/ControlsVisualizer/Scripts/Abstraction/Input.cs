using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LTX.ControlsVisualizer.Abstraction
{
    public struct Input
    {
        public string Path { get => _path; }
        public bool IsComposite { get => _isComposite; }
        public Input[] Children { get => _children; }
        public string DeviceLayout { get => _deviceLayout; }
        public string AdditionnalData { get => _additionnalData; set => _additionnalData = value; }

        private string _path;
        private string _deviceLayout;
        private bool _isComposite;
        private string _additionnalData;

        private Input[] _children;

        public Input(string deviceLayout, string path) : this()
        {
            _path = path;
            _deviceLayout = deviceLayout;
            _isComposite = false;
            _children = null;
        }

        public Input(string deviceLayout, string internalName, Input[] children) : this()
        {
            _path = internalName;
            _deviceLayout = deviceLayout;
            _children = children;
            _isComposite = true;
        }
    }
}
