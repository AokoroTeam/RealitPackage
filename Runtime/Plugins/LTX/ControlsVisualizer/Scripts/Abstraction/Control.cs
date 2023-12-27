using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LTX.ControlsVisualizer.Abstraction
{
    [System.Serializable]
    public struct Control
    {
        [SerializeField]
        internal bool enabled;
        [SerializeField]
        internal string name;
        [SerializeField]
        internal Command[] commands;
        [SerializeField]
        internal string[] layouts;

        public Control(string name, params Command[] commands)
        {
            this.name = name;
            this.commands = commands;

            List<string> layouts = new List<string>();
            for (int i = 0; i < commands.Length; i++)
            {
                string[] commandLayouts = commands[i].DeviceLayouts;
                for (int j = 0; j < commandLayouts.Length; j++)
                {
                    string l = commandLayouts[j];
                    if (!layouts.Contains(l))
                        layouts.Add(l);
                }
            }

            this.layouts = layouts.ToArray();
            enabled = true;
        }
    }
}
