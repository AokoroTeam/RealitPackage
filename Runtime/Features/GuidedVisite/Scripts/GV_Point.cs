using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Animations;

namespace Realit.Core.Features.GuidedVisite
{
    public class GV_Point : FeatureComponent<GuidedVisite>
    {
        public string PointName;

        [SerializeField]
        private TextMeshProUGUI label;
        [SerializeField]
        private Canvas canvas;
        [SerializeField]
        private LayerMask groundLayer;

        private Camera cam;
        public List<GV_Point> nearbyPoints;


        
        private void Start()
        {
            label.text = PointName;
            if(Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitInfo, 100, groundLayer))
                transform.position = hitInfo.point + Vector3.up * 0.001f;
        }

        private void LateUpdate()
        {
            if (cam != null)
            {
                label.transform.forward = -(cam.transform.position - transform.position);
            }
        }

        private void OnValidate()
        {
            if(!string.IsNullOrWhiteSpace(PointName))
            {
                gameObject.name = $"GV_{PointName}";
            }
        }
        private void OnDrawGizmosSelected()
        {
            foreach(GV_Point point in nearbyPoints)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, point.transform.position);
            }
        }

        public void ShowPoint()
        {
            canvas.gameObject.SetActive(true);
        }

        public void HidePoint()
        {
            canvas.gameObject.SetActive(false);
        }


        public void OnClick()
        {
            Feature.GoToPoint(this, false);
        }
        protected override void OnFeatureStarts()
        {
            gameObject.SetActive(true);
        }

        protected override void OnFeatureEnds()
        {
            gameObject.SetActive(false);
        }
        protected override void OnFeatureInitiate()
        {
            cam = Feature.overlayCamera.cam;
            canvas.worldCamera = cam;
        }

    }
}