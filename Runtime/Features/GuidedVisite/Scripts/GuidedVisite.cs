using LTX.ChanneledProperties;
using Realit.Core.Managers;
using Realit.Core.Player.CameraManagement;
using Realit.Core.Player.Controls;
using Realit.Core.Player.Movement;

using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;


namespace Realit.Core.Features.GuidedVisite
{

    [System.Serializable]
    public class GuidedVisite : Feature
    {
        public event Action<GV_Point> OnPointChanged;

        public GV_Point[] points;
        public Dictionary<InfoType, InfoRepresentation> infoRepresentations;


        private GV_Point _currentPoint;
        public GV_Point CurrentPoint
        {
            get
            {
                if (_currentPoint == null)
                    _currentPoint = points[0];

                return _currentPoint;
            }
        }

        public CameraControllerProfile lastPlayerProfile;
        public PlayerCharacter playerCharacter;
        public PlayerControls playerControls;
        public CameraManager cameraManager;
        public PlayerInput playerInputs;

        internal GV_CamOverlay overlayCamera;

        public GuidedVisite(FeatureDataAsset asset) : base(asset)
        {

        }

        #region Features events
        protected override void OnLoad()
        {
            GuidedVisite_Data _Data = Data as GuidedVisite_Data;

            //Points
            var ps = GameObject.FindObjectsByType<GV_Point>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            List<GV_Point> pointsList = new();
            for (int i = 0; i < ps.Length; i++)
            {
                if (ps[i].TryGetComponent(out GV_Point point))
                    pointsList.Add(point);
            }

            pointsList.Sort(
                (first, second) => 
                    first.transform.GetSiblingIndex().CompareTo(second.transform.GetSiblingIndex())
                );

            points = new GV_Point[pointsList.Count];
            pointsList.CopyTo(points);

            //Camera
            overlayCamera = GameObject.Instantiate(_Data.overlayCamera).GetComponent<GV_CamOverlay>();
            
            //UI
            RealitSceneManager.UI.CreateWindow(_Data.FeatureName, _Data.window);
            RealitSceneManager.UI.windowPriority.AddChannel(MyChannelKey, PriorityTags.None, Data.FeatureName);

            infoRepresentations = new();
            for (int i = 0; i < _Data.reprensentations.Length; i++)
            {
                var rep = _Data.reprensentations[i];
                if (!infoRepresentations.ContainsKey(rep.infoType))
                    infoRepresentations.Add(rep.infoType, rep);
            }

            //Cursor
            CursorManager.CursorLockMode.AddChannel(MyChannelKey, PriorityTags.None, CursorLockMode.Confined);
            CursorManager.CursorVisibility.AddChannel(MyChannelKey, PriorityTags.None, true);

            //Player
            Player.Realit_Player player = RealitSceneManager.Player;

            if (player.GetLivingComponent(out playerCharacter))
            {
                playerInputs = playerCharacter.Manager.playerInput;
                var actionMap = playerInputs.actions.FindActionMap("EV");

                InputAction lookAction = actionMap.FindAction("Look");
                lookAction.performed += OnLook;
                lookAction.canceled += OnLook;
                lookAction.started += OnLook;

                actionMap.FindAction("Next").performed += OnNext;
                actionMap.FindAction("Previous").performed += OnPrevious;
            }

            if (player.GetLivingComponent(out cameraManager))
            {
                cameraManager.XInput.AddChannel(MyChannelKey, PriorityTags.None);
                cameraManager.YInput.AddChannel(MyChannelKey, PriorityTags.None);
            }

            if(player.GetLivingComponent(out playerControls))
                playerControls.actionMapPriority.AddChannel(MyChannelKey, PriorityTags.None, "EV");
        }

        protected override void OnUnload()
        {
            points = null;

            CursorManager.CursorLockMode.RemoveChannel(MyChannelKey);
            CursorManager.CursorLockMode.RemoveChannel(MyChannelKey);

            
            RealitSceneManager.UI.windowPriority.RemoveChannel(MyChannelKey);
            RealitSceneManager.UI.DestroyWindow(Data.FeatureName);

            if (playerCharacter != null)
                playerCharacter.Freezed.RemoveChannel(MyChannelKey);


            if (playerControls != null)
                playerControls.actionMapPriority.RemoveChannel(MyChannelKey);
            
            if (playerInputs != null)
            {
                var actionMap = playerInputs.actions.FindActionMap("EV");

                InputAction lookAction = actionMap.FindAction("Look");
                lookAction.performed -= OnLook;
                lookAction.canceled -= OnLook;
                lookAction.started -= OnLook;

                actionMap.FindAction("Next").performed -= OnNext;
                actionMap.FindAction("Previous").performed -= OnPrevious;
            }

            if (cameraManager != null)
            {
                cameraManager.XInput.RemoveChannel(MyChannelKey);
                cameraManager.YInput.RemoveChannel(MyChannelKey);
            }

        }

        protected override void OnStart()
        { 
            Aokoro.UI.UIManager ui = RealitSceneManager.UI;

            if (!ui.windowPriority.ChangeChannelPriority(MyChannelKey, PriorityTags.High))
            {
                EndFeature();
                return;
            }

            CursorManager.CursorLockMode.ChangeChannelPriority(MyChannelKey, PriorityTags.High);
            CursorManager.CursorVisibility.ChangeChannelPriority(MyChannelKey, PriorityTags.High);

            if (playerCharacter != null)
            {
                playerCharacter.Freezed.ChangeChannelPriority(MyChannelKey, PriorityTags.High);
            }
            else
                LogWarning("PlayerCharacter is missing");

            if(playerControls != null)
            {
                playerControls.actionMapPriority.ChangeChannelPriority(MyChannelKey, PriorityTags.High);
            }
            else
                LogWarning("PlayerControls is missing");

            if (cameraManager != null)
            {
                lastPlayerProfile = cameraManager.CurrentProfile;
                cameraManager.SwitchToCameraProfile(GetData<GuidedVisite_Data>().profile);
                cameraManager.XInput.ChangeChannelPriority(MyChannelKey, 30);
                cameraManager.YInput.ChangeChannelPriority(MyChannelKey, 30);
            }
            else
                LogWarning("CameraManager is missing");


            GoToPoint(points[0], true);
        }

        protected override void OnEnd()
        {

            RealitSceneManager.UI.windowPriority.ChangeChannelPriority(MyChannelKey, PriorityTags.None);
            
            CursorManager.CursorLockMode.ChangeChannelPriority(MyChannelKey, PriorityTags.None);
            CursorManager.CursorLockMode.ChangeChannelPriority(MyChannelKey, PriorityTags.None);

            if (cameraManager != null)
                cameraManager.SwitchToCameraProfile(lastPlayerProfile);

            if (playerCharacter != null)
                playerCharacter.Freezed.ChangeChannelPriority(MyChannelKey, PriorityTags.None);
            
            if(playerControls != null)
                playerControls.actionMapPriority.ChangeChannelPriority(MyChannelKey, PriorityTags.None);
            
            if (cameraManager != null)
            {
                cameraManager.XInput.ChangeChannelPriority(MyChannelKey, PriorityTags.None);
                cameraManager.YInput.ChangeChannelPriority(MyChannelKey, PriorityTags.None);
            }
        }

        #endregion
        bool overUI;

        #region Inputs
        private void OnLook(InputAction.CallbackContext ctx)
        {
            switch (ctx.phase)
            {
                case InputActionPhase.Started:
                    overUI = Aokoro.UIExtentions.IsPointerOverUi(Pointer.current.position.value);
                    break;
                case InputActionPhase.Performed:
                    if (!overUI)
                    {
                        Vector2 input = ctx.ReadValue<Vector2>();
                        if (cameraManager.XInput.HasChannel(MyChannelKey))
                            cameraManager.XInput.Write(MyChannelKey, input.x);

                        if (cameraManager.YInput.HasChannel(MyChannelKey))
                            cameraManager.YInput.Write(MyChannelKey, input.y);
                    }
                    break;
                case InputActionPhase.Canceled:
                    overUI = false;
                    if (cameraManager.XInput.HasChannel(MyChannelKey))
                        cameraManager.XInput.Write(MyChannelKey, 0);

                    if (cameraManager.YInput.HasChannel(MyChannelKey))
                        cameraManager.YInput.Write(MyChannelKey, 0);
                    break;
            }
        }

        private void OnPrevious(InputAction.CallbackContext obj)
        {
            int previousPointIndex = -1;
            for (int i = 0; i < points.Length; i++)
            {
                if (points[i] == CurrentPoint)
                {
                    previousPointIndex = i == 0 ? points.Length - 1 : i - 1;
                    break;
                }
            }

            if(previousPointIndex != -1)
                GoToPoint(points[previousPointIndex], false);
        }

        private void OnNext(InputAction.CallbackContext obj)
        {
            int nextPointIndex = -1;
            for (int i = 0; i < points.Length; i++)
            {
                if (points[i] == CurrentPoint)
                {
                    nextPointIndex = i == points.Length - 1 ? 0 : i + 1;
                    break;
                }
            }

            if(nextPointIndex != -1)
                GoToPoint(points[nextPointIndex], false);
        }

        #endregion
        internal void GoToPoint(GV_Point point, bool immediate)
        {
            if(immediate)
                playerCharacter.SnapToPositionAsAgent(point.transform.position);
            else
                playerCharacter.StartAgentTravel(point.transform.position);

            _currentPoint = point;

            for (int i = 0; i < points.Length; i++)
            {
                var p = points[i];
                if (p != _currentPoint && _currentPoint.nearbyPoints.Contains(p))
                    p.ShowPoint();
                else
                    p.HidePoint();
            }

            OnPointChanged?.Invoke(point);
        }
    }
}
