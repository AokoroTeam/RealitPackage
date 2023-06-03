using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LTX.ControlsVisualizer.Abstraction
{
    public struct Control
    {
        private string name;
        private Command[] commands;

        public Control(string name, Command[] commands)
        {
            this.name = name;
            this.commands = commands;
        }
    }
}
