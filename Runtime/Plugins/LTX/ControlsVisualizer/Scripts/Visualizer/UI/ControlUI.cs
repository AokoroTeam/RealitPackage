using LTX.ControlsVisualizer.Abstraction;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LTX.ControlsVisualizer.UI
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

        protected abstract void OnFilledWithCommands();

        protected virtual void OnDestroy()
        {
            if(commands != null)
            {
                Clear();
            }
        }


        internal void FillWithControls(ControlUIContent controlUIContent)
        {
            Clear();
            Command[] commands = controlUIContent.control.commands;
            for (int i = 0; i < commands.Length; i++)
            {
                //if(controlUIContent.)
                //var commandUI = CreateCommandUI(controlUIContent);
                //this.commands.Add(commandUI);

                //commandUI.Visualizer = Visualizer;
                //commandUI.Internal_ApplyCommandData(command);
            }

            OnFilledWithCommands();
        }

        //public virtual CommandUI CreateCommandUI(ControlUIContent controlUIContent)
        //{

        //}
        protected virtual void Clear()
        {
            foreach (var command in commands)
                Destroy(command);

            commands.Clear();
        }
    }
}
