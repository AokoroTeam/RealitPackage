using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Realit.Core.Features.GuidedVisite
{
    public class GV_Point : MonoBehaviour
    {
        public string PointName;
        public List<GV_Point> nearbyPoints;

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
    }
}