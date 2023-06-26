using Lean.Gui;
using NaughtyAttributes;
using Realit.Core.Player;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;

namespace Realit.Core.Player.Controls
{
    public class MobileJoystick : OnScreenControl, MobileControls.IMobileControl
    {
        public event Action OnDragEvent;
        public event Action OnPointerDownEvent;
        public event Action OnPointerUpEvent;

        private LeanJoystick joystick;

        [InputControl(layout = "Vector2")]
        [BoxGroup("Move"), SerializeField]
        private string moveControl;
        [BoxGroup("Move"), ReadOnly]
        public bool IsHolding;
        protected override string controlPathInternal
        {
            get => moveControl;
            set => moveControl = value;
        }

        private void Awake()
        {
            joystick = GetComponentInChildren<LeanJoystick>();
        }

        public void OnUp()
        {
            IsHolding = false;
            OnPointerUpEvent?.Invoke();
        }

        public void OnDown()
        {
            OnPointerDownEvent?.Invoke();
            IsHolding = true;
        }
        
        void MobileControls.IMobileControl.Perform()
        {
            SendValueToControl(joystick.ScaledValue * .75f);
        }

        void MobileControls.IMobileControl.Enable(PlayerControls player)
        {
            joystick.interactable = true;
        }

        void MobileControls.IMobileControl.Disable(PlayerControls player)
        {
            joystick.interactable = false;
        }
    }
}
