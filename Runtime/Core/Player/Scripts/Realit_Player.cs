using Aokoro.Entities.Player;
using LTX.ChanneledProperties;

using NaughtyAttributes;

using Realit.Core.Features;
using Realit.Core.Managers;

using System;
using UnityEngine;
using UnityEngine.InputSystem;

using Screen = UnityEngine.Device.Screen;
using Application = UnityEngine.Device.Application;
using SystemInfo = UnityEngine.Device.SystemInfo;

namespace Realit.Core.Player
{
    public class Realit_Player : PlayerManager
    {
        public event Action<Realit_Player> OnControlChanges;

        [SerializeField, BoxGroup("Features")]
        private Transform featuresRoot;
        public Transform FeaturesRoot => featuresRoot;


        private bool controlsHaveChanged = false;
        protected override void Start()
        {
            base.Start();
            if(Application.isMobilePlatform)
                playerInput.SwitchCurrentControlScheme("Mobile");
            else
                playerInput.SwitchCurrentControlScheme("Keyboard&Mouse");

        }

        
        private void OnEnable()
        {
            controlsHaveChanged = true;
        }

        private void OnDisable()
        {
            controlsHaveChanged = false;
        }

        protected override void LateUpdate()
        {
            base.LateUpdate();
            if(controlsHaveChanged)
            {
                controlsHaveChanged = false;
                OnControlChanges?.Invoke(this);
            }
        }


        private void PlayerInput_onControlsChanged(PlayerInput obj) => controlsHaveChanged = true;

        protected override void SetupCursorForPlayer()
        {
            CursorManager.CursorLockMode.AddChannel(this, PriorityTags.Default, CursorLockMode.Locked);
            CursorManager.CursorVisibility.AddChannel(this, PriorityTags.Default, false);
        }

        
        public GameObject AddFeatureObject(GameObject source)
        {
            var instance = Instantiate(source, FeaturesRoot);
            var playerFeatures = instance.GetComponentsInChildren<IPlayerFeatureComponent>();
            for (int i = 0; i < playerFeatures.Length; i++)
                playerFeatures[i].Player = this;
            
            return instance;
        }
    }
}