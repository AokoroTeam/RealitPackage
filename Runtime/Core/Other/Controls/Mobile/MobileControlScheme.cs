using Realit.Core.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Realit.Core.Controls.MobileControls;

namespace Realit.Core.Controls
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

        public void EnableScheme(Realit_Player player)
        {
            for (int i = 0; i < controls.Length; i++)
                controls[i].Enable(player);

            canvasGroup.alpha = 1;
        }

        public void DisableScheme(Realit_Player player)
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
