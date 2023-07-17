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
        private CameraManager _cameraManager;
        private Queue<CameraControllerProfile> _cameraControllerProfiles;

        public Queue<CameraControllerProfile> CameraControllerProfiles 
        { 
            get
            {
                if(_cameraControllerProfiles == null)
                    _cameraControllerProfiles = new Queue<CameraControllerProfile>();

                if(_cameraControllerProfiles.Count == 0)
                {
                    if(_cameraManager != null)
                    {
                        foreach (var controller in _cameraManager.Controllers)
                        {
                            CameraControllerProfile profile = controller.Key;
                            _cameraControllerProfiles.Enqueue(profile);
                        }
                    }
                }

                return _cameraControllerProfiles;
            }
        }

        public CameraSwitch(FeatureDataAsset asset) : base(asset)
        {
        }


        protected override void OnStart()
        {
            if (_cameraManager != null)
            {
                CameraControllerProfile currentProfile = _cameraManager.CurrentProfile;
                CameraControllerProfile profile = currentProfile;

                while (profile == currentProfile)
                {
                    if (!CameraControllerProfiles.TryDequeue(out profile))
                        break;
                }

                _cameraManager.SwitchToCameraProfile(profile);
            }
            EndFeature();
        }

        protected override void OnEnd()
        {
        }

        protected override void OnLoad()
        {
            _cameraManager = Realit_Player.LocalPlayer.GetLivingComponent<CameraManager>();
        }


        protected override void OnUnload()
        {
        }

        internal void SelectCameraType(CameraControllerProfile profile)
        {
            if (_cameraManager != null)
                _cameraManager.SwitchToCameraProfile(profile);
        }
    }
}
