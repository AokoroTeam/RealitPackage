using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LTX.ControlsVisualizer.Abstraction;
using System;

namespace LTX.ControlsVisualizer
{
    public abstract class ControlProvider : MonoBehaviour
    {
        public event Action<ControlsFactory> onControlsChange;

        public ControlsFactorySettings settings;

        private ControlsFactory controlsFactory;
        public ControlsFactory ControlsFactory 
        { 
            get => controlsFactory??=new(settings); 
            set => controlsFactory = value; 
        }

        protected virtual void Awake()
        {
            Internal_FillControlFactory();
        }

        private void Internal_FillControlFactory()
        {
            ClearControlFactory();
            FillControlFactory(ControlsFactory);
            OnControlsChange();
        }

        protected abstract void FillControlFactory(ControlsFactory controlsFactory);
        protected void ClearControlFactory() => ControlsFactory.Clear();


        //Call manually
        protected void OnControlsChange() => onControlsChange?.Invoke(ControlsFactory);
    }
}
