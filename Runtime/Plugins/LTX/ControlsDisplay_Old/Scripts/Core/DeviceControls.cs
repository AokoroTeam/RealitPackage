using NaughtyAttributes;
using UnityEngine;

namespace LTX.ControlsDisplay
{
    [CreateAssetMenu(menuName = "Aokoro/UI/Inputs/DeviceControls")]
    public class DeviceControls : ScriptableObject
    {
        public string DeviceName => deviceName;

        [SerializeField, BoxGroup("Settings")]
        private string deviceName;
        [HorizontalLine]
        [SerializeField, BoxGroup("Controls")]
        private InputDisplay defaultControl;
        [SerializeField, BoxGroup("Controls")]
        private InputDisplay[] SpecialControls;

        internal InputRepresentation GetInputRepresentationFromControl(Control control)
        {
            InputDisplay display = FindDisplayForControl(control);
            return new InputRepresentation(display, control);
        }

        internal int GetInputRepresentationsFromControls(Control[] controls, InputRepresentation[] output)
        {
            int size = 0;
            for (int i = 0; i < controls.Length; i++)
            {
                Control control = controls[i];
                InputRepresentation representation = GetInputRepresentationFromControl(control);
                if (representation != null)
                {
                    output[size] = representation;
                    size++;
                }
            }

            return size;
        }

        private InputDisplay FindDisplayForControl(Control control)
        {
            for (int i = 0; i < SpecialControls.Length; i++)
            {
                InputDisplay controlData = SpecialControls[i];
                if (controlData.MatchesControl(control))
                    return controlData;

            }

            return defaultControl;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            for (int i = 0; i < SpecialControls.Length; i++)
                SpecialControls[i].Validate();
        }
#endif
    }
}
