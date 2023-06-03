using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Aokoro.Entities.Player;
using Aokoro.Entities;
using System;

using LTX.ControlsDisplay;

namespace Aokoro.UI.ControlsDiplay
{
    public class PlayerControls : MonoBehaviour, ILateUpdateEntityComponent<PlayerManager>
    {
        string IEntityComponent.ComponentName => "PlayerControls";

        public event System.Action OnControlChanges;
        private PlayerInput playerInput;
        public PlayerManager Manager { get; set; }

        private InputActionMap lastMap;
        private string lastScheme;

        public void Initiate(PlayerManager manager)
        {
            playerInput = manager.playerInput;
        }
        private void Awake()
        {
            playerInput = GetComponent<PlayerInput>();
            if (playerInput.actions != null)
                SetupInputDevices();
        }


        public void SetupInputDevices()
        {
            InputDevice[] devices = InputSystem.devices.ToArray();
            lastMap = playerInput.currentActionMap;
            playerInput.SwitchCurrentControlScheme(devices);
        }

        private void OnEnable()
        {
            playerInput.onControlsChanged += OnControlsChanges;
            playerInput.onDeviceLost += OnControlsChanges;
            playerInput.onDeviceRegained += OnControlsChanges;
        }

        private void OnDisable()
        {
            playerInput.onControlsChanged -= OnControlsChanges;
            playerInput.onDeviceLost -= OnControlsChanges;
            playerInput.onDeviceRegained -= OnControlsChanges;
        }
        private void OnControlsChanges(PlayerInput playerInput)
        {
            //Debug.Log("[Player] Controls have changed");
            if (lastScheme != playerInput.currentControlScheme)
            {
                lastScheme = playerInput.currentControlScheme;
                OnControlChanges?.Invoke();
            }
        }

        public void OnLateUpdate()
        {
            if (lastMap != playerInput.currentActionMap)
            {
                lastMap = playerInput.currentActionMap;
                OnControlsChanges(playerInput);
            }

        }
    }
}