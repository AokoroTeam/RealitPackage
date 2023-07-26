using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Realit.Core.Features.GuidedVisite
{
    [System.Serializable]
    public struct InfoRepresentation
    {
        public InfoType infoType;
        [ShowAssetPreview]
        public Sprite sprite;
        public string label;
    }
}
