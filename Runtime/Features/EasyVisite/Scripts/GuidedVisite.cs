using Aokoro;
using Realit.Core.Managers;
using Realit.Core.Player.CameraManagement;
using Realit.Core.Player.Movement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

namespace Realit.Core.Features.GuidedVisite
{

    [System.Serializable]
    public class GuidedVisite : Feature
    {
        public event Action<GV_Point> OnPointChanged;

        public GV_Point[] points;
        

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

        private CameraControllerProfile lastPlayerProfile;
        private PlayerCharacter playerCharacter;
        private CameraManager cameraManager;
        private PlayerInput playerInputs;

        public GuidedVisite(FeatureDataAsset asset) : base(asset)
        {

        }

        #region Features events
        protected override void OnLoad()
        {
            GuidedVisite_Data _Data = Data as GuidedVisite_Data;

            var ps = GameObject.FindGameObjectsWithTag("GV_Point");
            List<GV_Point> pointsList = new List<GV_Point>();
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
            RealitSceneManager.UI.CreateWindow(_Data.FeatureName, _Data.window);
            RealitSceneManager.UI.windowPriority.AddChannel(this, PriorityTags.None, Data.FeatureName);

            CursorManager.Instance.cursorLockMode.AddChannel(this, PriorityTags.None, CursorLockMode.Confined);
            CursorManager.Instance.cursorVisibility.AddChannel(this, PriorityTags.None, true);


            Player.Realit_Player player = RealitSceneManager.Player;

            if (player.GetLivingComponent(out PlayerCharacter character))
                character.Freezed.RemoveChannel(this);

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
                cameraManager.XInput.AddChannel(this, PriorityTags.None);
                cameraManager.YInput.AddChannel(this, PriorityTags.None);
            }

            player.actionMap.AddChannel(this, PriorityTags.None, "EV");
        }

        protected override void OnUnload()
        {
            points = null;
            CursorManager.Instance.cursorLockMode.RemoveChannel(this);
            CursorManager.Instance.cursorVisibility.RemoveChannel(this);

            
            RealitSceneManager.UI.windowPriority.RemoveChannel(this);
            RealitSceneManager.UI.DestroyWindow(Data.FeatureName);

            if (playerCharacter != null)
            {
                playerCharacter.Freezed.RemoveChannel(this);
                playerCharacter.Manager.actionMap.RemoveChannel(this);
            }

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
                cameraManager.XInput.RemoveChannel(this);
                cameraManager.YInput.RemoveChannel(this);
            }

        }

        protected override void OnStart()
        { 
            Aokoro.UI.UIManager ui = RealitSceneManager.UI;

            if (!ui.windowPriority.ChangeChannelPriority(this, PriorityTags.High))
            {
                EndFeature();
                return;
            }

            CursorManager cursor = CursorManager.Instance;

            cursor.cursorLockMode.ChangeChannelPriority(this, PriorityTags.High);
            cursor.cursorVisibility.ChangeChannelPriority(this, PriorityTags.High);


            if (cameraManager != null)
            {
                lastPlayerProfile = cameraManager.CurrentProfile;
                cameraManager.SwitchToCameraProfile(GetData<GuidedVisite_Data>().profile);
            }

            if (playerCharacter != null)
            {
                playerCharacter.Freezed.ChangeChannelPriority(this, PriorityTags.High);
                playerCharacter.Manager.actionMap.ChangeChannelPriority(this, PriorityTags.High);
            }

            if(cameraManager != null)
            {
                cameraManager.XInput.ChangeChannelPriority(this, PriorityTags.High + 1);
                cameraManager.YInput.ChangeChannelPriority(this, PriorityTags.High + 1);
            }

            
            GoToPoint(points[0], true);
        }

        protected override void OnEnd()
        {
            RealitSceneManager.UI.windowPriority.ChangeChannelPriority(this, PriorityTags.None);

            CursorManager cursor = CursorManager.Instance;
            cursor.cursorLockMode.ChangeChannelPriority(this, PriorityTags.None);
            cursor.cursorVisibility.ChangeChannelPriority(this, PriorityTags.None);

            if (cameraManager != null)
                cameraManager.SwitchToCameraProfile(lastPlayerProfile);

            if (playerCharacter != null)
            {
                playerCharacter.Freezed.ChangeChannelPriority(this, PriorityTags.None);
                playerCharacter.Manager.actionMap.ChangeChannelPriority(this, PriorityTags.None);
            }

            if (cameraManager != null)
            {
                cameraManager.XInput.ChangeChannelPriority(this, PriorityTags.None);
                cameraManager.YInput.ChangeChannelPriority(this, PriorityTags.None);
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
                    overUI = UIExtentions.IsPointerOverUi(Pointer.current.position.value);
                    break;
                case InputActionPhase.Performed:
                    if (!overUI)
                    {
                        Vector2 input = ctx.ReadValue<Vector2>();
                        if (cameraManager.XInput.HasChannelOwnBy(this))
                            cameraManager.XInput.Write(this, input.x);

                        if (cameraManager.YInput.HasChannelOwnBy(this))
                            cameraManager.YInput.Write(this, input.y);
                    }
                    break;
                case InputActionPhase.Canceled:
                    overUI = false;
                    if (cameraManager.XInput.HasChannelOwnBy(this))
                        cameraManager.XInput.Write(this, 0);

                    if (cameraManager.YInput.HasChannelOwnBy(this))
                        cameraManager.YInput.Write(this, 0);
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
            OnPointChanged?.Invoke(point);
        }
    }
}
