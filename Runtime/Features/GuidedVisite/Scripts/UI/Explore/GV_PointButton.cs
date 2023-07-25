using Michsky.MUIP;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Realit.Core.Features.GuidedVisite.UI
{
    public class GV_PointButton : MonoBehaviour
    {
        [SerializeField]
        ButtonManager buttonManager;

        [ReadOnly] internal GV_Point point;
        [ReadOnly] internal GV_PointList list;
        [ReadOnly] internal GuidedVisite EV;
        
        
        private void Awake()
        {
            list = GetComponentInParent<GV_PointList>();
        }

        internal void SetPoint(GV_Point point)
        {
            this.point = point;
            buttonManager.SetText(point.PointName);
        }

        public void OnClicked() => EV.GoToPoint(point, false);
    }
}
