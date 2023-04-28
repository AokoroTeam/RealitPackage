using Aokoro.UI.ControlsDiplaySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Realit.Core.Managers;

namespace Realit.Core.Player.Movement
{
    [AddComponentMenu("Realit/Reader/Player/Movement/UI")]
    public class PlayerMovementUI : ControlDisplayer
    {
        private void Awake()
        {
            RealitSceneManager.OnPlayerIsSetup += OnPlayerIsCreatedCallback;
        }

        private void OnPlayerIsCreatedCallback(Realit_Player player)
        {
            RealitSceneManager.OnPlayerIsSetup -= OnPlayerIsCreatedCallback;
            PlayerCharacter playerCharacter = player.GetLivingComponent<PlayerCharacter>();
            playerCharacter.UI = this;
            AssignActionProvider(playerCharacter, true);
        }
    }
}