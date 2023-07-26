using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using VisualDesignCafe.Nature.Overlay;

namespace Realit.Core.Features.GuidedVisite
{
    public class GV_CamOverlay : FeatureComponent<GuidedVisite>
    {
        [HideInInspector]
        private Camera mainCam;
        [HideInInspector]
        internal Camera cam;

        protected override void Awake()
        {
            base.Awake();
            cam = GetComponent<Camera>();
        }

        private void LateUpdate()
        {
            transform.position = mainCam.transform.position;
            transform.rotation = mainCam.transform.rotation;
            cam.fieldOfView = mainCam.fieldOfView;
        }

        protected override void OnFeatureInitiate()
        {
            mainCam = Camera.main;
        }

        protected override void OnFeatureStarts()
        {
            var data = mainCam.GetUniversalAdditionalCameraData();

            data.cameraStack.Add(cam);
            mainCam.UpdateVolumeStack(data);
            gameObject.SetActive(true);
        }

        protected override void OnFeatureEnds()
        {
            var data = mainCam.GetUniversalAdditionalCameraData();

            data.cameraStack.Remove(cam);
            mainCam.UpdateVolumeStack(data);
            gameObject.SetActive(false);
        }

    }
}
