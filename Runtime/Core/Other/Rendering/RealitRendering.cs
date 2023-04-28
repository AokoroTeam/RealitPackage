using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using NaughtyAttributes;

namespace Realit.Core
{
    public class RealitRendering : MonoBehaviour
    {
        public VolumeProfile highQualityPP;
        public VolumeProfile lowQualityPP;

        [SerializeField]
        private int targetFrameRate = 60;

        private void Awake()
        {
            Application.targetFrameRate = targetFrameRate;
        }
    }
}
