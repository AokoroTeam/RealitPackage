using LTX.ChanneledProperties;
using Realit.Core.Features.UI;
using Realit.Core.Player.CameraManagement;
using Realit.Core.Player;
using Realit.Core.Player.Movement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Realit.Core.Features.Running
{
    public class Running : Feature
    {
        private bool isRunning;
        private ChannelKey channelKey;

        private PlayerCharacter playerCharacter;

        public Running(FeatureDataAsset asset) : base(asset)
        {
            channelKey = ChannelKey.GetUniqueChannelKey(asset);

        }

        protected override void OnStart()
        {
            playerCharacter.autoSprint.Write(channelKey, true);
            isRunning = true;
            UpdateSprite();
        }
        protected override void OnEnd()
        {
            playerCharacter.autoSprint.Write(channelKey, false);
            isRunning = false;
            UpdateSprite();
        }
        protected override void OnLoad()
        {

            playerCharacter = Realit_Player.LocalPlayer.GetLivingComponent<PlayerCharacter>();
            playerCharacter.autoSprint.AddChannel(channelKey, PriorityTags.Small);
        }

        protected override void OnUnload()
        {
            playerCharacter.autoSprint.RemoveChannel(channelKey);
        }

        public void UpdateSprite()
        {

            if (FeaturesManager.UI.TryGetUIForFeature(this, out FeatureIndicator icon))
            {
                Running_Data running_Data = (Data as Running_Data);
                icon.SetIcon(isRunning ? running_Data.run : running_Data.walk);
            }
        }
    }
}
