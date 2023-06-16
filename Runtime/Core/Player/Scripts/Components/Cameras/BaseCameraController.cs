using Cinemachine;
using LTX.Settings;
using NaughtyAttributes;
using System;
using UnityEngine;

namespace Realit.Core.Player.CameraManagement
{
    public abstract class BaseCameraController : MonoBehaviour, AxisState.IInputAxisProvider
    {
        [ReadOnly, BoxGroup("Infos")]
        public CameraManager cameraManager;
        [BoxGroup("Infos"), Required]
        public CameraControllerProfile Profile;
        
        [BoxGroup("Overrides")]
        [SerializeField, Range(0, 5)]
        private float xModifier = 1;
        [BoxGroup("Overrides")]
        [SerializeField, Range(0, 5)]
        private float yModifier = 1;
        [BoxGroup("Overrides")]
        [SerializeField, Range(0, 5)]
        private float zModifier = 1;


        protected virtual void OnValidate()
        {
            if (Profile != null)
                gameObject.name = Profile.name;
        }

        public virtual void Recenter(Vector3 forward, float damp = -1)
        {
            if (damp < 0)
                transform.forward = forward;
            else
                transform.forward = Vector3.Lerp(transform.forward, forward, damp * Time.deltaTime);
        }
        
        public virtual float GetAxisValue(int axis)
        {
            if (enabled && cameraManager != null)
            {
                float value = 0;
                float floatSettingValue;
                bool boolSettingValue;
                switch (axis)
                {
                    case 0:
                        value = cameraManager.X * xModifier;
                        if (MainSettingsManager.TryGetSettingValue("CamXSensitivity", out floatSettingValue))
                            value *= floatSettingValue;
                        if (MainSettingsManager.TryGetSettingValue("CamXInvert", out boolSettingValue) && boolSettingValue)
                            value *= -1;
                        return value;
                    case 1:
                         value = cameraManager.Y * yModifier;
                        if (MainSettingsManager.TryGetSettingValue("CamYSensitivity", out floatSettingValue))
                            value *= floatSettingValue;
                        if (MainSettingsManager.TryGetSettingValue("CamYInvert", out boolSettingValue) && boolSettingValue)
                            value *= -1;

                        return value;
                    case 2:
                        value = cameraManager.Z * zModifier;
                        return value;
                }
            }

            return 0;
        }

        public abstract void OnUpdate();

    }
}
