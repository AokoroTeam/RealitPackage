using LTX.ControlsVisualizer.Abstraction;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LTX.ControlsVisualizer.UI
{
    public abstract class CommandUI : MonoBehaviour
    {
        [SerializeField]
        Transform commandRepParent;

        List<InputUI> inputs = new List<InputUI>();

        public ControlsVisualizerUI Visualizer
        {
            get;
            internal set;
        }

        internal void Internal_ApplyCommandData(CommandUIContent data)
        {
            
        }

        protected abstract void ApplyCommandData(CommandUIContent data);
    }
}
