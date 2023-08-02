using Realit.Core.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Realit.Core.Player.Controls.MobileControls;

namespace Realit.Core.Player.Controls
{
    public class MobileControlScheme : MonoBehaviour
    {
        public string SchemeName => gameObject.name;

        private IMobileControl[] controls;
        private CanvasGroup canvasGroup;


        private void Awake()
        {
            controls = GetComponentsInChildren<IMobileControl>();
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public void EnableScheme(PlayerControls player)
        {
            for (int i = 0; i < controls.Length; i++)
                controls[i].Enable(player);

            canvasGroup.alpha = 1;
        }

        public void DisableScheme(PlayerControls player)
        {
            for (int i = 0; i < controls.Length; i++)
                controls[i].Disable(player);
            
            canvasGroup.alpha = 0;
        }

        internal void PerformControls()
        {
            for (int i = 0; i < controls.Length; i++)
                controls[i].Perform();
        }
    }
}
