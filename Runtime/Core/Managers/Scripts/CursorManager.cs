using LTX;
using LTX.ChanneledProperties;
using NaughtyAttributes;
using UnityEngine;

namespace Realit.Core.Managers
{
    public class CursorManager : Singleton<CursorManager>
    {
        public static PrioritisedProperty<CursorLockMode> CursorLockMode
        {
            get
            {
                if (Instance.cursorLockMode == null)
                    Instance.cursorLockMode = new PrioritisedProperty<CursorLockMode>();

                return Instance.cursorLockMode;
            }
        }

        public static PrioritisedProperty<bool> CursorVisibility
        {
            get
            {
                if(Instance.cursorVisibility == null)
                    Instance.cursorVisibility = new PrioritisedProperty<bool>();

                return Instance.cursorVisibility;
            }
        }

        [SerializeField, ReadOnly]
        private PrioritisedProperty<CursorLockMode> cursorLockMode;
        [SerializeField, ReadOnly]
        private PrioritisedProperty<bool> cursorVisibility;

        private void CursorVisibility_OnValueChanged(bool value) => Cursor.visible = value;
        private void CursorLockMode_OnValueChanged(CursorLockMode value) => Cursor.lockState = value;

        private void OnEnable()
        {
            CursorLockMode.AddOnValueChangeCallback(CursorLockMode_OnValueChanged);
            CursorVisibility.AddOnValueChangeCallback(CursorVisibility_OnValueChanged);
        }

        private void OnDisable()
        {
            CursorLockMode.RemoveOnValueChangeCallback(CursorLockMode_OnValueChanged);
            CursorVisibility.RemoveOnValueChangeCallback(CursorVisibility_OnValueChanged);
        }

        protected override void OnExistingInstanceFound(CursorManager existingInstance)
        {
            Destroy(gameObject);
        }
    }
}