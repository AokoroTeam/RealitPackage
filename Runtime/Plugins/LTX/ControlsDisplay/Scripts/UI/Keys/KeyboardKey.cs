using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace LTX.ControlsDisplay.UI
{
    public class KeyboardKey : ControlIcon
    {
        [SerializeField]
        TextMeshProUGUI text;

        private const string numpad = "numpad";
        public override void SetupIcon(Control control)
        {
            string displayName = control.DisplayName;
            string path = control.Path;

            if (path.StartsWith(numpad))
                text.SetText(path.Remove(0, numpad.Length));
            else
            {
                if (int.TryParse(path, out _))
                    text.SetText(path);
                else
                    text.SetText(displayName);
            }
        }
    }
}