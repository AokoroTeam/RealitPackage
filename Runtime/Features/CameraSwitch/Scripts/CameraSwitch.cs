using Realit.Core.Managers;
using Realit.Core.Player;
using Realit.Core.Player.CameraManagement;
using Realit.Core.Player.Movement;
using System;
using System.Collections.Generic;
using LTX.ChanneledProperties;

namespace Realit.Core.Features.CameraSwitch
{

    [System.Serializable]
    public class CameraSwitch : Feature
    {
        public CameraControllerProfile currentProfile;
        public readonly List<CameraSwitchProfile> associatedProfiles;

        public CameraSwitch(FeatureDataAsset asset) : base(asset)
        {
            associatedProfiles = new List<CameraSwitchProfile>();
            var d = asset as CameraSwitch_Data;
            foreach(var p in d.profiles)
            {
                associatedProfiles.Add(p);
                //LogMessage($"{p.profile.name} added to list");
            }
        }


        protected override void OnStart()
        {
            var d = Data as CameraSwitch_Data;
            if (Realit_Player.LocalPlayer.GetLivingComponent(out CameraManager manager))
                currentProfile = manager.CurrentProfile;

            CursorManager.Instance.cursorLockMode.ChangeChannelPriority(this, PriorityTags.Highest);
            CursorManager.Instance.cursorVisibility.ChangeChannelPriority(this, PriorityTags.Highest);

            RealitSceneManager.UI.windowPriority.ChangeChannelPriority(this, PriorityTags.Highest);
            GameNotifications.Instance.canUpdate.ChangeChannelPriority(this, PriorityTags.Highest);
        }

        protected override void OnEnd()
        {

            CursorManager.Instance.cursorLockMode.ChangeChannelPriority(this, PriorityTags.None);
            CursorManager.Instance.cursorVisibility.ChangeChannelPriority(this, PriorityTags.None);

            RealitSceneManager.UI.windowPriority.ChangeChannelPriority(this, PriorityTags.None);
            GameNotifications.Instance.canUpdate.ChangeChannelPriority(this, PriorityTags.None);
        }

        protected override void OnLoad()
        {
            CameraSwitch_Data _Data = Data as CameraSwitch_Data;
        
            RealitSceneManager.UI.CreateWindow(_Data.windowName, _Data.window);

            GameNotifications.Instance.canUpdate.AddChannel(this, PriorityTags.None, false);
            RealitSceneManager.UI.windowPriority.AddChannel(this, PriorityTags.None, _Data.windowName);

            CursorManager.Instance.cursorLockMode.AddChannel(this, PriorityTags.None, UnityEngine.CursorLockMode.None);
            CursorManager.Instance.cursorVisibility.AddChannel(this, PriorityTags.None, true);

        }


        protected override void OnUnload()
        {
            CameraSwitch_Data _Data = Data as CameraSwitch_Data;

            RealitSceneManager.UI.DestroyWindow(_Data.windowName);

            GameNotifications.Instance.canUpdate.RemoveChannel(this);
            RealitSceneManager.UI.windowPriority.RemoveChannel(this);

            CursorManager.Instance.cursorLockMode.RemoveChannel(this);
            CursorManager.Instance.cursorVisibility.RemoveChannel(this);

        }

        internal void SelectCameraType(CameraControllerProfile profile)
        {
            if (Realit_Player.LocalPlayer.GetLivingComponent(out CameraManager cameraManager))
                cameraManager.SwitchToCameraProfile(profile);
        }
    }
}
