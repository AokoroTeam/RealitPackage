using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;

using LTX.ControlsVisualizer;
using LTX.ChanneledProperties;
using LTX.Entities.Player;
using LTX.Entities;

using NaughtyAttributes;

using Screen = UnityEngine.Device.Screen;
using Application = UnityEngine.Device.Application;
using SystemInfo = UnityEngine.Device.SystemInfo;


namespace Realit.Core.Player.Controls
    
{
    [System.Serializable]
    public class PlayerControlsProvider : ControlProvider
    {
        public PlayerInput playerInput;
        
        public PlayerControlsProvider(PlayerInput playerInput)
        {
            this.playerInput = playerInput;
            
            ClearControlFactory();
            
            FillControlFactory(ControlsFactory);

            OnControlsChange();
        }

        protected override void FillControlFactory(ControlsFactory controlsFactory)
        {
            var actionMaps = playerInput.actions.actionMaps;
            var devices = playerInput.devices.ToArray();
            foreach (var actionMap in actionMaps)
            {
                var actions = actionMap.actions;
                foreach (var action in actions)
                    controlsFactory.AddControl(action, devices);
            }
        }
    }

    [AddComponentMenu("Realit/Player/PlayerControls")]
    public class PlayerControls : MonoBehaviour, ILateUpdateEntityComponent<PlayerManager>
    {
        public PlayerControlsProvider controlsProvider;

        string IEntityComponent.ComponentName => "PlayerControls";
        int IEntityComponent.InitialisationPriority => 10;

        public event Action<string, string> OnMapChange;
        public event Action<PlayerControls> OnControlChanges;

        [ReadOnly, BoxGroup("Action maps")]
        public PrioritisedProperty<string> actionMapPriority;

        [BoxGroup("Action maps")]
        public string DefaultActionMap;

        [ReadOnly, BoxGroup("Refresh")]
        private bool controlsHaveChanged = false;

        public PlayerInput PlayerInput { get; private set; }
        public PlayerManager Manager { get; set; }

        public void Initiate(PlayerManager manager)
        {
            PlayerInput = manager.playerInput;
            controlsProvider = new PlayerControlsProvider(PlayerInput);
            
            manager.Freezed.AddOnValueChangeCallback(OnFreezed);
        }

        private void Awake()
        {
            actionMapPriority = new PrioritisedProperty<string>(DefaultActionMap);
            actionMapPriority.AddOnValueChangeCallback(ChangeActionMap);

            PlayerInput = GetComponent<PlayerInput>();
            if (PlayerInput.actions != null)
                SetupInputDevices();
        }

        private void OnFreezed(bool freezed)
        {
            if (freezed)
                PlayerInput.DeactivateInput();
            else
                PlayerInput.ActivateInput();
        }

        private void Start()
        {
            ChangeActionMap(actionMapPriority);
            PlayerInput.currentActionMap?.Enable();
        }

        private void OnEnable()
        {
            PlayerInput.onControlsChanged += OnControlsChanges;
            PlayerInput.onDeviceLost += OnControlsChanges;
            PlayerInput.onDeviceRegained += OnControlsChanges;
            controlsHaveChanged = true;
        }

        private void OnDisable()
        {
            PlayerInput.onControlsChanged -= OnControlsChanges;
            PlayerInput.onDeviceLost -= OnControlsChanges;
            PlayerInput.onDeviceRegained -= OnControlsChanges;
            controlsHaveChanged = false; 
        }

        public void OnLateUpdate()
        {
            if (controlsHaveChanged)
            {
                controlsHaveChanged = false;
                OnControlChanges?.Invoke(this);
                Debug.Log("[Realit Player] Controls have changed");
            }
        }
        public void SetupInputDevices()
        {
            InputDevice[] devices = InputSystem.devices.ToArray();
            string controlScheme = Application.isMobilePlatform ? "Mobile" : "Keyboard&Mouse";
            PlayerInput.SwitchCurrentControlScheme(controlScheme, devices);
        }
        protected void SetupInputs()
        {
            PlayerInput.ActivateInput();

            var actionMaps = PlayerInput.actions.actionMaps;
            foreach (var actionMap in actionMaps)
                actionMap.Disable();
        }

        public void ChangeActionMap(string targetMap)
        {

            if (PlayerInput.actions != null)
            {
                if (PlayerInput.actions.FindActionMap(targetMap) != null)
                {
                    InputActionMap lastActionMap = PlayerInput.currentActionMap;
                    lastActionMap?.Disable();

                    if (lastActionMap != null)
                    {
                        string currentMap = lastActionMap.name;
                        OnMapChange?.Invoke(currentMap, targetMap);
                    }

                    PlayerInput.SwitchCurrentActionMap(targetMap);
                    //currentMap = playerInput.currentActionMap;

                    Debug.Log($"[Realit Player] Changing action map to {PlayerInput.currentActionMap.name}");
                }
            }
        }

        private void OnControlsChanges(PlayerInput obj) => controlsHaveChanged = true;
    }
}