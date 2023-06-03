using UnityEngine;

namespace LTX.ControlsDisplay
{
    [CreateAssetMenu(menuName = "Aokoro/UI/Inputs/DeviceControlsList")]
    internal class Data : ScriptableObject
    {
        internal GameObject Or => or;
        internal GameObject And => and;

        [SerializeField]
        private GameObject or;
        [SerializeField]
        private GameObject and;
        [SerializeField]
        private ControlScheme[] deviceControls;

        internal bool TryGetControlsForScheme(string schemeName, out ControlScheme scheme)
        {
            for (int i = 0; i < deviceControls.Length; i++)
            {
                ControlScheme cD_ControlScheme = deviceControls[i];

                if (cD_ControlScheme.ControlSchemeName == schemeName)
                {
                    scheme = cD_ControlScheme;
                    return true;
                }
            }

            scheme = default;
            return false;

        }

    }
}
