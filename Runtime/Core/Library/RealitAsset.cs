using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using Aokoro.ModelExports.Runtime;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Realit.Library
{

    [AddComponentMenu("Realit/Library/RealitPrefab"), ExecuteInEditMode]
    public class RealitAsset : MonoBehaviour
    {
#if UNITY_EDITOR
        [ReadOnly]
        public string projectPath;
#endif

        [SerializeField, ReadOnly]
        public string addressableKey;

        public RealitAsset OnAssetCreated(string addressableKey)
        {
            this.addressableKey = addressableKey;

            if (TryGetComponent(out ModelExportComponent modelExporter))
            {
                if (Application.isPlaying)
                    DestroyImmediate(modelExporter);
                else
                    Destroy(modelExporter);
            }

            return this;
        }
    }

}