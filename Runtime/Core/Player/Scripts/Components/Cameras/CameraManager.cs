using LTX.Entities;
using LTX.Entities.Player;
using LTX.ChanneledProperties;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

namespace Realit.Core.Player.CameraManagement
{
    public class CameraManager : MonoBehaviour, IEntityComponent<RealitPlayer>, ILateUpdateEntityComponent
    {
        /// <summary>
        /// First BaseCameraController : From
        /// Second BaseCameraController : To
        /// </summary>
        public event Action<BaseCameraController, BaseCameraController> OnCameraProfileChanged;
        [BoxGroup("Base settings")]
        [SerializeField, Range(.01f, 4)]
        private float verticalSpeed = 1;
        [BoxGroup("Base settings")]
        [SerializeField, Range(.01f, 4)]
        private float horizontalSpeed = 1;
        [BoxGroup("Base settings")]
        [SerializeField, Range(.01f, 4)]
        private float zoomSpeed = 1;
        [BoxGroup("Default")]
        [SerializeField, ReadOnly]
        InputAction XYaxis;
        [BoxGroup("Default")]
        [SerializeField, ReadOnly]
        InputAction ZAxis;


        [BoxGroup("Axis")]
        public PrioritisedProperty<float> XInput = new(0);
        [BoxGroup("Axis")]
        public PrioritisedProperty<float> YInput = new(0);
        [BoxGroup("Axis")]
        public PrioritisedProperty<float> ZInput = new(0);
        [BoxGroup("Axis"), HorizontalLine]
        public PrioritisedProperty<bool> XActive = new(true);
        [BoxGroup("Axis")]
        public PrioritisedProperty<bool> YActive = new(true);
        [BoxGroup("Axis")]
        public PrioritisedProperty<bool> ZActive = new(true);


        public float X => XInput * horizontalSpeed;
        public float Y => YInput * verticalSpeed;
        public float Z => ZInput * zoomSpeed;

        public RealitPlayer Manager { get; set; }

        string IEntityComponent.ComponentName => "CameraManager";
        int IEntityComponent.InitialisationPriority => 0;

        public CameraControllerProfile CurrentProfile { get; set; }

        public bool HasCameraController => CurrentProfile != null && Controllers.ContainsKey(CurrentProfile);
        public BaseCameraController CurrentCameraController => HasCameraController ? Controllers[CurrentProfile] : null;

        public Dictionary<CameraControllerProfile, BaseCameraController> Controllers
        {
            get;
            private set;
        }

#if UNITY_EDITOR
        [SerializeField, ReadOnly]
        List<BaseCameraController> controllers;

        private void OnValidate()
        {
            controllers = new List<BaseCameraController>(GetComponentsInChildren<BaseCameraController>(true));
        }
#endif
        private void Awake()
        {
            this.Controllers = new();
        }

        private void Start()
        {
            if (Controllers.Count > 0)
                SwitchToCameraProfile(Controllers.FirstOrDefault().Key);
        }


        public void Initiate(RealitPlayer manager)
        {
            BaseCameraController[] controllers = GetComponentsInChildren<BaseCameraController>(true);

            for (int i = 0; i < controllers.Length; i++)
                AddController(controllers[i]);

#if UNITY_EDITOR
            this.controllers = new(controllers);
#endif

            EnableInputs();
        }

        public bool SwitchToCameraProfile(CameraControllerProfile cameraProfile)
        {
            if (Controllers.TryGetValue(cameraProfile, out BaseCameraController next))
            {
                BaseCameraController last = CurrentProfile != null && Controllers.ContainsKey(CurrentProfile) ? Controllers[CurrentProfile] : null;
                CurrentProfile = cameraProfile;

                foreach (var kvp in Controllers)
                    kvp.Value.gameObject.SetActive(kvp.Key == cameraProfile);

                OnCameraProfileChanged?.Invoke(last, next);
                return true;
            }

            Debug.LogWarning($"Couldn't find {cameraProfile} inside array");
            return false;
        }

        private void OnEnable()
        {
            EnableInputs();
        }


        private void OnDisable()
        {
            DisableInputs();
        }

        public virtual void DisableInputs()
        {
            XYaxis?.Disable();
        }

        public virtual void EnableInputs()
        {
            if (Manager != null)
            {
                var map = Manager.playerInput.actions.FindActionMap("DefaultGameplay");
                XYaxis = map.FindAction("Look");
                ZAxis = map.FindAction("Zoom");

                if (!XInput.HasChannel(this))
                    XInput.AddChannel(this, PriorityTags.Smallest);
                if (!YInput.HasChannel(this))
                    YInput.AddChannel(this, PriorityTags.Smallest);
                if (!ZInput.HasChannel(this))
                    ZInput.AddChannel(this, PriorityTags.Smallest);

                if (!XActive.HasChannel(this))
                    XActive.AddChannel(this, PriorityTags.Smallest, true);
                if (!YActive.HasChannel(this))
                    YActive.AddChannel(this, PriorityTags.Smallest, true);
                if (!ZActive.HasChannel(this))
                    ZActive.AddChannel(this, PriorityTags.Smallest, true);
            }
        }


        public void OnLateUpdate()
        {
            Vector2 xy = XYaxis != null ? XYaxis.ReadValue<Vector2>() : Vector2.zero;
            float z = ZAxis != null ? ZAxis.ReadValue<float>() : 0.0f;

            XInput.Write(this, XActive ? xy.x : 0);
            YInput.Write(this, YActive ? xy.y : 0);
            ZInput.Write(this, ZActive ? z : 0);

            if(CurrentCameraController != null)
                CurrentCameraController.OnUpdate();
        }


        public void AddController(BaseCameraController controller)
        {
            if (!Controllers.ContainsKey(controller.Profile))
            {
                controller.cameraManager = this;
                Controllers.Add(controller.Profile, controller);
#if UNITY_EDITOR
                controllers.Add(controller);
#endif

                controller.transform.SetParent(PlayerManager.PlayerContainer.transform);
                controller.Recenter(Realit.Instance.Spawn.forward);
            }
            else
                Debug.LogWarning(
                    $"Disabling {controller.gameObject.name} because an other camera with the same profile is already registered",
                    controller);

            controller.gameObject.SetActive(false);

        }

        public void RemoveController(BaseCameraController controller)
        {
            if (Controllers.Remove(controller.Profile))
            {
                controller.cameraManager = null;
#if UNITY_EDITOR
                controllers.Remove(controller);
#endif
                //controller.transform.SetParent(null);

                controller.gameObject.SetActive(false);
                if (CurrentCameraController == controller && Controllers.Count > 0)
                    SwitchToCameraProfile(Controllers.FirstOrDefault().Key);
            }
        }
    }
}
