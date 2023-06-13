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

        public override void RecenterSmooth(Vector3 forward, float damp = -1)
        {

        }
    }
}