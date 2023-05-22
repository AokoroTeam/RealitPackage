using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Realit.Core.Controls.Modules
{
    public class Arrow : MobileBaseLookSurface
    {
        [SerializeField]
        RectTransform dot;
        [SerializeField]
        RectTransform arrow;
        [SerializeField]
        CanvasGroup canvasGroup;
        [SerializeField]
        float maxSize = 10f;


        Vector2 startPosition;

        
    }
}
