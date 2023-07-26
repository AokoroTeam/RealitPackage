using NaughtyAttributes;
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
        [SerializeField, BoxGroup("Base")]
        public string PointName;
        [SerializeField, BoxGroup("Base")]
        private TextMeshProUGUI label;
        [SerializeField, BoxGroup("Base")]
        private Canvas canvas;
        [SerializeField, BoxGroup("Base")]
        private LayerMask groundLayer;
        [SerializeField, BoxGroup("Base")]
        public List<GV_Point> nearbyPoints;

        [SerializeField, BoxGroup("Data")]
        public List<Info> infos = new List<Info>();

        private Camera cam;

        protected override void Awake()
        {
            base.Awake();
            gameObject.SetActive(false);
        }

        

        private void Start()
        {
            label.text = PointName;
            if(Physics.Raycast(transform.position + Vector3.up, Vector3.down, out RaycastHit hitInfo, 100, groundLayer))
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

            infos.Sort((first, second) => first.type.CompareTo(second.type));
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
            gameObject.SetActive(true);
            HidePoint();
            cam = Feature.overlayCamera.cam;
            canvas.worldCamera = cam;
        }

    }
}