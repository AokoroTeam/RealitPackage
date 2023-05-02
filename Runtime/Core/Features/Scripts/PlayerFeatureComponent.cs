using Aokoro.Entities.Player;
using Aokoro.UI.ControlsDiplay;
using LTX.ControlsDisplay;

using Realit.Core.Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Realit.Core.Features
{

    public abstract class PlayerFeatureComponent<T> : FeatureComponent<T>, IPlayerFeatureComponent, IInputActionsProvider, IPlayerInputAssetProvider where T : Feature
    {
        public event System.Action OnActionsNeedRefresh;
        
        [SerializeField] 
        private string mapName;
        [SerializeField] 
        private InputActionAsset actions;

        public string MapName => mapName;
        public Realit_Player Player { get; set; }
        public InputActionAsset ActionAsset { get => actions; set => actions = value; }


        private PlayerControls playerControls;

        protected override void Awake()
        {
            base.Awake();
            playerControls = GetComponentInParent<PlayerControls>();
        }


        protected virtual void OnEnable()
        {
            playerControls.OnControlChanges += TriggerRefresh;
        }

        protected virtual void OnDisable()
        {
            playerControls.OnControlChanges -= TriggerRefresh;
        }
        private void TriggerRefresh() => OnActionsNeedRefresh?.Invoke();

        public abstract void BindToNewActions(InputActionAsset actions);



        #region Interfaces

        InputAction[] IInputActionsProvider.GetInputActions() => ActionAsset.actionMaps[0].actions.ToArray();

        string IInputActionsProvider.GetControlScheme() => Player.playerInput.currentControlScheme;

        public InputDevice[] GetDevices() => Player.playerInput.devices.ToArray();
        #endregion
    }
}