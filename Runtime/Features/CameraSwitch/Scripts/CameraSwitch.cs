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

            CursorManager.Instance.cursorLockMode.ChangeChannelPriority(MyChannelKey, PriorityTags.Highest);
            CursorManager.Instance.cursorVisibility.ChangeChannelPriority(MyChannelKey, PriorityTags.Highest);

            RealitSceneManager.UI.windowPriority.ChangeChannelPriority(MyChannelKey, PriorityTags.Highest);
            GameNotifications.Instance.canUpdate.ChangeChannelPriority(MyChannelKey, PriorityTags.Highest);
        }

        protected override void OnEnd()
        {

            CursorManager.Instance.cursorLockMode.ChangeChannelPriority(MyChannelKey, PriorityTags.None);
            CursorManager.Instance.cursorVisibility.ChangeChannelPriority(MyChannelKey, PriorityTags.None);

            RealitSceneManager.UI.windowPriority.ChangeChannelPriority(MyChannelKey, PriorityTags.None);
            GameNotifications.Instance.canUpdate.ChangeChannelPriority(MyChannelKey, PriorityTags.None);
        }

        protected override void OnLoad()
        {
            CameraSwitch_Data _Data = Data as CameraSwitch_Data;
        
            RealitSceneManager.UI.CreateWindow(_Data.windowName, _Data.window);

            GameNotifications.Instance.canUpdate.AddChannel(MyChannelKey, PriorityTags.None, false);
            RealitSceneManager.UI.windowPriority.AddChannel(MyChannelKey, PriorityTags.None, _Data.windowName);

            CursorManager.Instance.cursorLockMode.AddChannel(MyChannelKey, PriorityTags.None, UnityEngine.CursorLockMode.None);
            CursorManager.Instance.cursorVisibility.AddChannel(MyChannelKey, PriorityTags.None, true);

        }


        protected override void OnUnload()
        {
            CameraSwitch_Data _Data = Data as CameraSwitch_Data;

            RealitSceneManager.UI.DestroyWindow(_Data.windowName);

            GameNotifications.Instance.canUpdate.RemoveChannel(MyChannelKey);
            RealitSceneManager.UI.windowPriority.RemoveChannel(MyChannelKey);

            CursorManager.Instance.cursorLockMode.RemoveChannel(MyChannelKey);
            CursorManager.Instance.cursorVisibility.RemoveChannel(MyChannelKey);

        }

        internal void SelectCameraType(CameraControllerProfile profile)
        {
            if (Realit_Player.LocalPlayer.GetLivingComponent(out CameraManager cameraManager))
                cameraManager.SwitchToCameraProfile(profile);
        }
    }
}
