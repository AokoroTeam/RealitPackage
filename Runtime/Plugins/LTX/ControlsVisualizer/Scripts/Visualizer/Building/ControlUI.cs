using LTX.ControlsVisualizer.Abstraction;
using LTX.ControlsVisualizer.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LTX.ControlsVisualizer
{
    public abstract class ControlUI : MonoBehaviour
    {
        [SerializeField]
        protected Transform commandsParent;

        protected List<CommandUI> commands = new List<CommandUI>();


        public ControlsVisualizerUI Visualizer
        {
            get;
            internal set;
        }

        internal void FillWithCommands(List<CommandUIData> datas)
        {
            Clear();

            foreach (var data in datas)
            {
                var commandUI = Visualizer.CreateCommandUI(commandsParent);
                commands.Add(commandUI);

                commandUI.Visualizer = Visualizer;
                commandUI.Internal_ApplyCommandData(data);
            }

            OnFilledWithCommands();
        }

        protected abstract void OnFilledWithCommands();

        protected virtual void OnDestroy()
        {
            if(commands != null)
            {
                Clear();
            }
        }

        protected virtual void Clear()
        {
            foreach (var command in commands)
                Destroy(command);

            commands.Clear();
        }
    }
}
