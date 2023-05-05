using Aokoro.Entities.Player;
using LTX.ChanneledProperties;

using NaughtyAttributes;

using Realit.Core.Features;
using Realit.Core.Managers;

using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Realit.Core.Player
{
    public class Realit_Player : PlayerManager
    {
        public event Action<Realit_Player> OnControlChanges;

        [SerializeField, BoxGroup("Features")]
        private Transform featuresRoot;
        public Transform FeaturesRoot => featuresRoot;


        
        private void OnEnable()
        {
            playerInput.onControlsChanged += PlayerInput_onControlsChanged;
        }

        private void OnDisable()
        {
            playerInput.onControlsChanged -= PlayerInput_onControlsChanged;
        }

        protected override void LateUpdate()
        {
            base.LateUpdate();
            /*
            Touchscreen touchscreen = Touchscreen.current;
            Keyboard keyboard = Keyboard.current;
            Mouse mouse = Mouse.current;
            Gamepad gamepad = Gamepad.current;

            if (touchscreen != null && touchscreen.wasUpdatedThisFrame)
            {
                if (playerInput.currentControlScheme != "Mobile")
                    playerInput.SwitchCurrentControlScheme("Mobile", touchscreen, gamepad);

                //Debug.Log("To mobile");
                return;
            }


            if (keyboard != null && mouse != null && 
                (keyboard.wasUpdatedThisFrame || mouse.leftButton.IsPressed() || mouse.rightButton.IsPressed())
                )
            {
                if (playerInput.currentControlScheme != "Keyboard&Mouse")
                    playerInput.SwitchCurrentControlScheme("Keyboard&Mouse", keyboard, mouse);
                
                //Debug.Log("To pc");
                return;
            }


            if (gamepad != null && gamepad.wasUpdatedThisFrame && !gamepad.description.empty)
            {
                if (playerInput.currentControlScheme != "Gamepad")
                    playerInput.SwitchCurrentControlScheme("Gamepad", gamepad);
                //Debug.Log("To gamepad");
                return;
            }
            */
        }


        private void PlayerInput_onControlsChanged(PlayerInput obj) => OnControlChanges?.Invoke(this);

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

        private void OnDestroy()
        {
            //Debug.Log("Player has been destroyed");
        }
    }
}