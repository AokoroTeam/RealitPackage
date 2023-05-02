using LTX.ControlsDisplay;
using Realit.Core.Managers;
using UnityEngine;

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