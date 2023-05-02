
using UnityEngine;
using UnityEngine.InputSystem;

namespace LTX.ControlsDisplay
{
    [System.Serializable]
    public struct ControlScheme
    {
        public string ControlSchemeName => controlSchemeName;

        [SerializeField] private string controlSchemeName;
        [SerializeField] private DeviceControls[] controls;

        public InputRepresentation GetInputRepresentationFromControl(Control control)
        {
            for (int i = 0; i < controls.Length; i++)
            {
                if (InputSystem.IsFirstLayoutBasedOnSecond(control.Device, controls[i].DeviceName))
                    return controls[i].GetInputRepresentationFromControl(control);
            }

            return null;
        }

        internal int GetInputRepresentationsFromControls(Control[] controls, InputRepresentation[] output)
        {
            int size = 0;
            for (int i = 0; i < controls.Length; i++)
            {
                InputRepresentation representation = GetInputRepresentationFromControl(controls[i]);
                if (representation != null)
                {
                    output[size] = representation;
                    size++;
                }
            }

            return size;
        }
    }
}