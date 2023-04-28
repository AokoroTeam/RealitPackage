using Aokoro.Entities.Player;
using Cinemachine;
using UnityEngine;
using Aokoro.Entities;
using Cinemachine.Utility;
using NaughtyAttributes;
using UnityEngine.InputSystem;
using Aokoro;
using System;
using UnityEngine.Windows;

namespace Realit.Core.Player.CameraManagement
{

    public class FirstPersonCamera : BaseCameraController
    {
        protected CinemachineVirtualCamera Vcam
        {
            get
            {
                if (_vcam == null)
                    _vcam = GetComponent<CinemachineVirtualCamera>();
                return _vcam;
            }
        }
        protected CinemachinePOV Pov
        {
            get
            {
                if (_pov == null)
                    _pov = Vcam.GetCinemachineComponent<CinemachinePOV>();
                return _pov;
            }
        }

        protected CinemachineRecomposer Recomposer
        {
            get
            {
                if (_composer == null)
                    _composer = GetComponent<CinemachineRecomposer>();
                return _composer;
            }
        }
        CinemachineVirtualCamera _vcam;
        CinemachinePOV _pov;
        CinemachineRecomposer _composer;

        [BoxGroup("Zoom")]
        [AxisStateProperty]
        public AxisState m_ZoomAxis = new AxisState(.35f, 1, false, false, 300f, 0.1f, 0.1f, "Zoom", true);

        [BoxGroup("Zoom")]
        [Tooltip("Controls how automatic recentering of the Vertical axis is accomplished")]
        public AxisState.Recentering m_ZoomAxisRecentering = new AxisState.Recentering(false, 1, 2);


        protected override void OnValidate()
        {
            base.OnValidate();
            m_ZoomAxis.Validate();
        }

        private void Awake()
        {
            m_ZoomAxis.SetInputAxisProvider(2, this);
            m_ZoomAxis.Value = 1;
        }

        private void OnDisable()
        {
            m_ZoomAxis.Value = 0;
        }

        public override void RecenterSmooth(Vector3 direction, float damp = -1)
        {
            if (damp >= 0)
            {
                Vector3 up = Pov.VcamState.ReferenceUp;
                Vector3 fwd = Vector3.forward;

                Transform parent = Pov.VirtualCamera.transform.parent;
                if (parent != null)
                    fwd = parent.rotation * fwd;

                Vector3 right = Vector3.Cross(up, fwd);

                Quaternion axis1 = Quaternion.AngleAxis(Pov.m_HorizontalAxis.Value, up);
                Quaternion axis2 = Quaternion.AngleAxis(Pov.m_VerticalAxis.Value, right);

                Quaternion camRot = axis1 * axis2;
                Vector3 camForward = camRot * fwd;

                Vector3 smoothRot = Vector3.Lerp(camForward, direction, damp * Time.deltaTime);
                //Debug.DrawRay(transform.position, direction);

                Pov.ForceCameraPosition(Vcam.transform.position, Quaternion.LookRotation(smoothRot));
            }
            else
            {
                Quaternion rot = Quaternion.LookRotation(direction, Vector3.up);
                Pov.ForceCameraPosition(Vcam.transform.position, rot);
            }
        }

        public override void OnUpdate()
        {
            if (CinemachineCore.Instance.IsLive(Pov.VirtualCamera))
            {
                float lastcurrentTime = CinemachineCore.CurrentTimeOverride;
                try
                {
                    CinemachineCore.CurrentTimeOverride = Time.realtimeSinceStartup;

                    if (m_ZoomAxis.Update(Time.deltaTime))
                        m_ZoomAxisRecentering.CancelRecentering();

                    m_ZoomAxisRecentering.DoRecentering(ref m_ZoomAxis, Time.deltaTime, 1);

                    if(Recomposer != null)
                        Recomposer.m_ZoomScale = m_ZoomAxis.Value;
                }
                finally
                {
                    CinemachineCore.CurrentTimeOverride = lastcurrentTime;
                }
            }
        }
    }
}