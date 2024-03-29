using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.InputSystem;

namespace LTX.ControlsDisplay
{
    public interface IInputActionsProvider
    {
        public InputAction[] GetInputActions();
        InputDevice[] GetDevices();
        public string GetControlScheme();
        public event System.Action OnActionsNeedRefresh;

    }
}
