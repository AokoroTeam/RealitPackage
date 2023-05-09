using Cinemachine;
using Realit.Core.Managers;
using Realit.Core.Player.CameraManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Realit.Core.Features.GuidedVisite
{
    public class GV_CamController : FirstPersonCamera
    {
        CinemachineVirtualCamera vcam;

        private void Awake()
        {
            vcam = GetComponent<CinemachineVirtualCamera>();

            if(RealitSceneManager.Player.TryGetComponent(out CameraManager manager))
                manager.AddController(this);

            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            if (cameraManager != null)
                cameraManager.RemoveController(this);

        }

        internal void SetFollowPosition(Transform follow)
        {
            vcam.Follow = follow;
        }
    }
}
