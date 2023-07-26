using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

namespace Realit.Core.Features.GuidedVisite.UI
{
    [AddComponentMenu("Realit/Reader/Features/EasyVisite/PointList")]
    public class GV_PointList : FeatureComponent<GuidedVisite>
    {
        [SerializeField]
        GameObject pointButtonPrefab;
        [SerializeField]
        Transform content;
        [SerializeField]
        GV_Selector selector;

        [SerializeField, ReadOnly]
        private GV_PointButton[] buttons;

        internal void CreateUI(GV_Point[] points)
        {
            int length = points.Length;
            buttons = new GV_PointButton[length];

            for (int i = 0; i < length; i++)
            {
                var go = Instantiate(pointButtonPrefab, content);
                var pointButton = go.GetComponent<GV_PointButton>();
                buttons[i] = pointButton;

                pointButton.EV = Feature;
                pointButton.SetPoint(points[i]);
            }

            selector.transform.SetAsLastSibling();
        }
        private void OnEnable()
        {
            if (Feature != null)
            {
                SelectCorrespondingButton(Feature.CurrentPoint);
            }
        }
        internal void SelectCorrespondingButton(GV_Point point)
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                if (buttons[i].point == point)
                {
                    selector.SetSelectedButton(buttons[i]);
                    return;
                }
            }
        }

        protected override void OnFeatureInitiate()
        {
            CreateUI(Feature.points);
            Feature.OnPointChanged += SelectCorrespondingButton;
        }


        protected override void OnFeatureStarts()
        {
            
        }

        protected override void OnFeatureEnds()
        {

        }

        private void OnDestroy()
        {
            if(Feature != null)
                Feature.OnPointChanged -= SelectCorrespondingButton;
        }
    }
}
