using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LTX.ControlsVisualizer
{
    public struct ControlsFactorySettings
    {
        public const int DefaultMaxCommand = 8;

        public static ControlsFactorySettings DefaultSettings = new();
        
        public int MaxCommand
        {
            get { return HasMaxCommand? maxCommand : DefaultMaxCommand; }
            set { maxCommand = value; } 
        }

        [SerializeField]
        private bool HasMaxCommand;
        [SerializeField]
        private int maxCommand;

        public string[] SupportedLayout
        { 
            get => supportedLayouts; 
            set => supportedLayouts = value; 
        }

        [SerializeField]
        private bool HasSupportedLayouts;
        [SerializeField]
        private string[] supportedLayouts;

    }
}
