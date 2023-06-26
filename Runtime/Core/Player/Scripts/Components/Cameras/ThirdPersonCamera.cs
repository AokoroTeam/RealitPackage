using Aokoro.Entities.Player;
using Aokoro.Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Aokoro;
using Cinemachine;
using NaughtyAttributes;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

namespace Realit.Core.Player.CameraManagement
{
    public class ThirdPersonCamera : BaseCameraController
    {
        CinemachineFreeLook FreeLook
        {
            get
            {
                if (_freeLook == null)
                    _freeLook = GetComponent<CinemachineFreeLook>();
                return _freeLook;
            }
        }

        CinemachineRecomposer Recomposer
        {
            get
            {
                if (_composer == null)
                    _composer = GetComponent<CinemachineRecomposer>();
                return _composer;
            }
        }


        CinemachineFreeLook _freeLook;
        CinemachineRecomposer _composer;


        public override void OnUpdate()
        {
            
        }

        public override void Recenter(Vector3 forward, float damp = -1)
        {
            var offset = -forward.normalized;
            Debug.DrawLine(transform.position, FreeLook.LookAt.position + offset);

            FreeLook.ForceCameraPosition(FreeLook.LookAt.position + offset, Quaternion.LookRotation(forward, Vector3.up));
        }

        public void OnCameraLive(ICinemachineCamera from, ICinemachineCamera to)
        {
            Recenter(from.VirtualCameraGameObject.transform.forward);
            FreeLook.m_YAxis.Value = .65f;
        }
    }
}
