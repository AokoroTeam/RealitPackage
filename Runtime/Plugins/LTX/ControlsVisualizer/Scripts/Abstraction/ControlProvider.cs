using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LTX.ControlsVisualizer.Abstraction;
using System;
using NaughtyAttributes;

namespace LTX.ControlsVisualizer
{
    [Serializable]
    public abstract class ControlProvider
    {
        public event Action<ControlsFactory> onControlsChange;

        public ControlsFactorySettings settings;

        [SerializeField, ReadOnly]
        private ControlsFactory controlsFactory;
        public ControlsFactory ControlsFactory 
        { 
            get => controlsFactory??=new(settings); 
            set => controlsFactory = value; 
        }
        public string[] CurrentLayouts { get; internal set; }

        protected abstract void FillControlFactory(ControlsFactory controlsFactory);
        protected void ClearControlFactory() => ControlsFactory.Clear();


        //Call manually
        protected void OnControlsChange() => onControlsChange?.Invoke(ControlsFactory);
    }
}
