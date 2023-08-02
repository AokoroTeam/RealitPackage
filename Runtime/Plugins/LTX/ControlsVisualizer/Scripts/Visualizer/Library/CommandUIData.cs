using LTX.ControlsVisualizer.Abstraction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LTX.ControlsVisualizer.UI
{
    public readonly struct CommandUIData
    {
        public readonly Command command;
        public readonly GameObject commandVisual;
        public readonly Dictionary<int, InputVisual> inputVisuals;

        public CommandUIData(Command command, GameObject commandVisual) : this()
        {
            this.command = command;
            this.commandVisual = commandVisual;

            inputVisuals = new();
        }
    }
}
