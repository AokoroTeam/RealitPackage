using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LTX.ControlsDisplay.UI
{
    public class MouseKey : ControlIcon
    {
        private Image image;
        private Image Image
        {
            get
            {
                if (image == null)
                    image = GetComponent<Image>();

                return image;
            }
        }

        [SerializeField]
        private Sprite lmb;
        [SerializeField]
        private Sprite rmb;
        [SerializeField]
        private Sprite mmb;



        public override void SetupIcon(Control control)
        {
            Image.sprite = control.Path switch
            {
                "leftButton" => lmb,
                "rightButton" => rmb,
                "middleButton" => mmb,
                _ => mmb,
            };
        }
    }
}