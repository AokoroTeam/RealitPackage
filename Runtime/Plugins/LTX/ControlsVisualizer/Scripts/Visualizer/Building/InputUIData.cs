using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LTX.ControlsVisualizer
{
    public readonly struct InputUIData
    {
        public readonly GameObject prefab;
        public readonly string path;
        public readonly string data;

        internal InputUIData(GameObject prefab, string path, string data) : this()
        {
            this.prefab = prefab;
            this.path = path;
            this.data = data;
        }
    }
}
