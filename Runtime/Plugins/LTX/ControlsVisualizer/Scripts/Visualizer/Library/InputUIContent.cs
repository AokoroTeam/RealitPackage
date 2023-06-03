using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LTX.ControlsVisualizer
{
    [System.Serializable]
    internal struct InputUIContent
    {
        [SerializeField]
        private string[] paths;

        [SerializeField]
        private GameObject visual;
        [SerializeField]
        private bool IsComposite;

        internal InputUIData GetVisual(string path, string additionnalData = null) => new(visual, path, additionnalData);

        internal bool Matches(string path)
        {
            for (int i = 0; i < paths.Length; i++)
            {
                if (path == paths[i])
                    return true;
            }

            return false;
        }

    }
}
