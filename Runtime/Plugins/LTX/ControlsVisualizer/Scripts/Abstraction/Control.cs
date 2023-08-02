using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LTX.ControlsVisualizer.Abstraction
{
    public struct Control
    {
        internal string name;
        internal Command[] commands;

        public Control(string name, params Command[] commands)
        {
            this.name = name;
            this.commands = commands;
        }
    }
}
