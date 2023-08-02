using Michsky.MUIP;
using Realit.Core.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.UI;

namespace Realit.Core.Player.Controls
{
    public class MobileRunSwitch : OnScreenControl, MobileControls.IMobileControl
    {

        [SerializeField, InputControl(layout = "Button")]
        private string moveControl;
        [SerializeField]
        private SwitchManager switchManager;
        [SerializeField]
        private Sprite onSprite, offSprite;
        [SerializeField]
        private Image icon;

        protected override string controlPathInternal
        {
            get => moveControl;
            set => moveControl = value;
        }
        private void Awake()
        {
            OnSliderChange(switchManager.isOn);
        }


        public void OnSliderChange(bool value)
        {
            icon.sprite = value ? onSprite : offSprite;
        }

        void MobileControls.IMobileControl.Perform()
        {
            SendValueToControl(switchManager.isOn ? 1f : 0f);
        }

        void MobileControls.IMobileControl.Enable(PlayerControls player)
        {
        }

        void MobileControls.IMobileControl.Disable(PlayerControls player)
        {
        }
    }
}
