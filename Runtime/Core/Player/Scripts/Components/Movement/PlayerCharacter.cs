using LTX.Entities;
using LTX.Entities.Player;

using LTX.ChanneledProperties;
using LTX.Settings;

using Realit.Core.Managers;
using Realit.Core.Player.CameraManagement;

using EasyCharacterMovement;
using NaughtyAttributes;
using System;

using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using Application = UnityEngine.Device.Application;

using RotationMode = EasyCharacterMovement.RotationMode;

namespace Realit.Core.Player.Movement
{
    [AddComponentMenu("Realit/Player/PlayerCharacter")]
    public class PlayerCharacter : Character, IEntityComponent<PlayerManager>
    {
        private const string AutoMovementWindow = "automovement";
        
        //Serialized fields
        [BoxGroup("ChanneledProperties"), ReadOnly]
        public ChanneledProperty<bool> Freezed;
        [BoxGroup("ChanneledProperties"), ReadOnly]
        public ChanneledProperty<Vector2> movementInput;
        [BoxGroup("ChanneledProperties"), ReadOnly]
        public ChanneledProperty<bool> autoSprint;
        [BoxGroup("ChanneledProperties"), ReadOnly]
        public ChanneledProperty<RotationMode> rotationMode;
        
        [Space]

        [BoxGroup("Navigation")]
        [SerializeField] GameObject agentMovingWindow;
        [BoxGroup("Navigation")]
        [ReadOnly] public bool isAgentMoving;
        [BoxGroup("Navigation")]
        [SerializeField] float cameraSmoothness;
        [BoxGroup("Navigation")]
        [SerializeField] float lookAheadDistance;
        [BoxGroup("Navigation"), Range(0,1)]
        [SerializeField] float YLookAheadAngle;
        [BoxGroup("Navigation")]
        [SerializeField, ReadOnly] Vector3[] pathCorners;

        //Properties
        [ShowNativeProperty]
        public CameraControllerProfile CurrentCameraProfile => camManager == null ? default : camManager.CurrentProfile;

        //Events
        public event System.Action OnActionsNeedRefresh;
        
        public event System.Action OnAgentStartsMoving;
        public event System.Action OnAgentStopsMoving;
        
        //Entity
        public PlayerManager Manager { get; set; }
        string IEntityComponent.ComponentName => "PlayerCharacter";
        int IEntityComponent.InitialisationPriority => 0;
        
        //Components
        private NavMeshAgent agent;
        private SkinnedMeshRenderer skinnedMeshRenderer;
        private CameraManager camManager;

        #region Unity Events
        protected override void OnAwake()
        {
            base.OnAwake();
            agent = GetComponent<NavMeshAgent>();
            camManager = GetComponentInChildren<CameraManager>();
            skinnedMeshRenderer = animator.GetComponentInChildren<SkinnedMeshRenderer>();

            isAgentMoving = false;
            agent.updatePosition = false;
            agent.updateRotation = false;

            camManager.OnCameraProfileChanged += OnCameraProfileChanged;

            movementInput = new(Vector2.zero);
            rotationMode = new(GetRotationMode());
            rotationMode.OnValueChanged += SetRotationMode;

            autoSprint = new(false);
        }

        protected override void OnStart()
        {
            base.OnStart();
            RealitSceneManager.UI.windowPriority.AddChannel(this, PriorityTags.None, AutoMovementWindow);
        }
        private void OnDestroy()
        {
            RealitSceneManager.UI.windowPriority.RemoveChannel(this);
        }
        protected override void OnOnEnable()
        {
            base.OnOnEnable();

            movementInput.AddChannel(this, PriorityTags.Smallest);
        }

        protected override void OnOnDisable()
        {
            base.OnOnDisable();
            
            movementInput.RemoveChannel(this);
        }

        protected override void OnUpdate()
        {
            if (isAgentMoving)
            {
                //Debug.DrawLine(transform.position, agent.destination);

                RotateTowards(agent.velocity.normalized, true);
                //Debug.Log(agent.remainingDistance);
                if (!agent.pathPending)
                {
                    if (agent.remainingDistance <= agent.stoppingDistance
                        || !agent.hasPath
                        || agent.pathStatus == NavMeshPathStatus.PathPartial
                        || agent.pathStatus == NavMeshPathStatus.PathInvalid)

                        OnAgentArrived();
                    else
                    {
                        SetPosition(agent.nextPosition, false);
                        var v = LookAheadOfPath();

                        camManager.XInput.Write(agent, v.x);
                        camManager.YInput.Write(agent, v.y);
                    }
                }

                Animate();
            }

            if (!Freezed)
                base.OnUpdate();
        }

        protected override void OnFixedUpdate()
        {
            if (!Freezed)
                base.OnFixedUpdate();
        }

        protected override void OnLateFixedUpdate()
        {
            if (!Freezed)
                base.OnLateFixedUpdate();

        }

        protected override void OnLateUpdate()
        {
            if (!Freezed)
                base.OnLateUpdate();
        }
        #endregion

        public void Initiate(PlayerManager manager)
        {
            inputActions = manager.playerInput.actions;
            camera = Camera.main;

            RealitSceneManager.UI.CreateWindow(AutoMovementWindow, agentMovingWindow);
            if (!Application.isMobilePlatform)
            {
                GameNotifications.Instance.AddNotificationToQueue("Conseil", "Nous vous conseillons de vous munir d'une souris pour pouvoir vous déplacer en même temps que vous regarder.", 10, 10);
                GameNotifications.Instance.AddNotificationToQueue("Conseil", "Vous pouvez courir en maintenant : \n - <b>Shift</b> \n - <b>clique gauche</b>.", 10, 12);
            }
            else
            {
                GameNotifications.Instance.AddNotificationToQueue("Conseil", "<b>Déplacez-vous<b> avec le </b>joystick</b> en bas à gauche de l'écran. \n <b>Regarder</b> autour de vous en <b>glissant</b> votre doigt sur l'écran.", 10, 2);
            }

            manager.OnEntityInitiated += Manager_OnEntityInitiated;
        }

        private void Manager_OnEntityInitiated(Entity entity)
        {
            if (!entity.GetLivingComponent(out camManager))
                Debug.LogWarning("[Player Character] Couldn't access player camera controller");
        }

        protected override void Animate()
        {
            if (animator)
            {
                Vector3 currentVelocity = GetVelocity();
                float speed = GetSpeed();

                if (GetMovementInput().sqrMagnitude > .1f)
                {
                    var normVelocity = transform.InverseTransformDirection(currentVelocity).normalized;
                    animator.SetFloat("Norm_Forward_Speed", normVelocity.z, .1f, Time.deltaTime);
                    animator.SetFloat("Norm_Right_Speed", normVelocity.x, .1f, Time.deltaTime);
                    //animator.SetFloat("Angular_Speed", AngularVelocity.y);
                }

                animator.SetBool("IsMoving", speed > .05f);
                animator.SetBool("IsRunning", IsSprinting());
                animator.SetFloat("Speed", speed);
            }
        }

        protected override Vector2 GetMovementInput()
        {
            movementInput.Write(this, base.GetMovementInput());

            return movementInput;
        }
        public override bool IsSprinting()
        {
            return canEverSprint && (_sprintButtonPressed || autoSprint);
        }

        public override float GetMaxSpeed()
        {
            float speed = base.GetMaxSpeed();

            if(MainSettingsManager.HasInstance)
            {
                if (IsSprinting() && MainSettingsManager.TryGetSettingValue("PlayerRunningSpeed", out float value))
                {
                    speed *= value;
                }

                else if (IsWalking() && MainSettingsManager.TryGetSettingValue("PlayerWalkingSpeed", out value))
                {
                    speed *= value;
                }
            }

            return speed;
        }

        public override Vector3 GetVelocity()
        {
            if (isAgentMoving)
                return agent.velocity;

            return base.GetVelocity();
        }

        #region Nav
        public void MoveToScreenLocationAsAgent(Vector2 screenPoint)
        {
            Ray ray = Camera.main.ScreenPointToRay(screenPoint);

            if (Physics.Raycast(ray, out RaycastHit info, Mathf.Infinity, characterMovement.collisionLayers))
            {
                if (NavMesh.SamplePosition(info.point, out NavMeshHit hit, 1, NavMesh.AllAreas))
                {
                    //Debug
                    //Debug.DrawRay(hit.position, Vector3.up * 5, Color.cyan, 5);
                    //Debug.DrawRay(ray.origin, ray.direction * 500, Color.cyan, 5);
                    //Debug.DrawLine(ray.origin, hit.position, Color.red, 5);

                    StartAgentTravel(hit.position);
                }
            }
        }

        public void SnapToPositionAsAgent(Vector3 destination)
        {
            if (Physics.Raycast(destination, Vector3.down, out RaycastHit rh, 100, characterMovement.collisionLayers))
            {
                if (NavMesh.SamplePosition(rh.point, out NavMeshHit nh, 1, NavMesh.AllAreas))
                {
                    characterMovement.SetPosition(nh.position);
                    agent.Warp(nh.position);
                }
            }
        }

        public void StartAgentTravel(Vector3 destination)
        {
            agent.Warp(transform.position);
            NavMeshPath path = new();

            if (NavMesh.CalculatePath(transform.position, destination, agent.areaMask, path))
            {
                if (agent.SetPath(path))
                {
                    isAgentMoving = true;
                    //agent.updatePosition = true;
                    //agent.updateRotation = true;
                    Freezed.AddChannel(agent, PriorityTags.VeryHigh, true);

                    //Camera
                    if (camManager != null)
                    {
                        camManager.XInput.AddChannel(agent, PriorityTags.VeryHigh);
                        camManager.YInput.AddChannel(agent, PriorityTags.VeryHigh);

                        camManager.XActive.AddChannel(agent, PriorityTags.VeryHigh, true);
                        camManager.YActive.AddChannel(agent, PriorityTags.VeryHigh, true);
                    }
                    
                    pathCorners = agent.path.corners;

                    RealitSceneManager.UI.windowPriority.ChangeChannelPriority(this, PriorityTags.Default);
                    OnAgentStartsMoving?.Invoke();
                    
                    Debug.Log($"[Realit Player] Agent going to {destination}");
                }
            }
            else
            {
                Debug.Log($"[Realit Player] Agent cannot go to {destination}");
                Debug.DrawLine(transform.position, destination, Color.red, 4);
            }
        }

        private Vector2 LookAheadOfPath()
        {
            
            if (agent.remainingDistance > lookAheadDistance && pathCorners != null)
            {
                int cornerCount = agent.path.GetCornersNonAlloc(pathCorners);

                float currentDistance = 0;

                for (int i = 0; i < cornerCount - 1; i++)
                {
                    Vector3 A = pathCorners[i];
                    Vector3 B = pathCorners[i + 1];

                    float distance = (B - A).magnitude;

                    if (currentDistance + distance >= lookAheadDistance)
                    {
                        float diff = (currentDistance + distance) - lookAheadDistance;

                        float normalizedPositionOnSegment = Mathf.InverseLerp(0, distance, diff);

                        Vector3 point = Vector3.Lerp(B, A, normalizedPositionOnSegment);
                        LookAt(point);
                        break;
                    }
                    else
                        currentDistance += distance;
                }
            }
            else
            {
                Vector3 toDest = agent.destination - transform.position;

                LookAt(toDest.sqrMagnitude <= .1f ? transform.position + transform.forward : agent.destination);
            }

            return Vector2.zero;
        }

        private void LookAt(Vector3 point)
        {
            Vector3 toPoint = point - transform.position;
            Vector3 flatToPoint = new Vector3(toPoint.x, 0, toPoint.z);

            float dot = Vector3.Dot(toPoint.normalized, flatToPoint.normalized);

            float yLerp = Mathf.InverseLerp(1 - YLookAheadAngle, 1, dot);

            Vector3 lookAtPoint = point - Camera.main.transform.position;

            if(camManager.HasCameraController)
                camManager.CurrentCameraController.Recenter(Vector3.Lerp(lookAtPoint, flatToPoint, yLerp), cameraSmoothness);
        }

        public void OnAgentArrived()
        {
            pathCorners = null;
            agent.ResetPath();
            agent.updatePosition = false;
            //agent.updateRotation = false;
            isAgentMoving = false;

            //Dependances
            Freezed.RemoveChannel(agent);

            if (camManager != null)
            {
                camManager.XInput.RemoveChannel(agent);
                camManager.YInput.RemoveChannel(agent);

                camManager.XActive.RemoveChannel(agent);
                camManager.YActive.RemoveChannel(agent);
            }

            OnAgentStopsMoving?.Invoke();

            RealitSceneManager.UI.windowPriority.ChangeChannelPriority(this, PriorityTags.None);
            Debug.Log("[Realit Player] Agent as stopped");
        }

        public void SkipAgentTravel()
        {
            if(isAgentMoving)
            {
                var destination = agent.destination;
                SnapToPositionAsAgent(destination);

                OnAgentArrived();
            }
        }

        #endregion

        public void OnCameraProfileChanged(BaseCameraController from, BaseCameraController to)
        {
            if (camManager)
            {
                //Every specific behavior
                switch (camManager.CurrentProfile.Tag)
                {
                    //First person camera
                    case "FC":
                        //SetRotationMode(RotationMode.OrientToCameraViewDirection);
                        SetRotationMode(RotationMode.OrientToMovement);
                        skinnedMeshRenderer.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
                        break;
                    //Third person camera
                    case "TC":
                        SetRotationMode(RotationMode.OrientToMovement);
                        
                        if(from != null)
                            RotateTowards(from.transform.forward, true);

                        skinnedMeshRenderer.shadowCastingMode = ShadowCastingMode.On;
                        break;
                    //Shoulder camera
                    case "SC":
                        //SetRotationMode(RotationMode.OrientToCameraViewDirection);
                        SetRotationMode(RotationMode.OrientToMovement);
                        skinnedMeshRenderer.shadowCastingMode = ShadowCastingMode.On;
                        break;
                }
            }
        }
    }
}