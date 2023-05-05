using LTX;
using LTX.ChanneledProperties;
using UnityEngine;

namespace Realit.Core.Managers
{
    public class CursorManager : Singleton<CursorManager>
    {
        public static ChanneledProperty<CursorLockMode> CursorLockMode
        {
            get
            {
                if (Instance.cursorLockMode == null)
                    Instance.cursorLockMode = new ChanneledProperty<CursorLockMode>();

                return Instance.cursorLockMode;
            }
        }

        public static ChanneledProperty<bool> CursorVisibility
        {
            get
            {
                if(Instance.cursorVisibility == null)
                    Instance.cursorVisibility = new ChanneledProperty<bool>();

                return Instance.cursorVisibility;
            }
        }

        private ChanneledProperty<CursorLockMode> cursorLockMode;
        private ChanneledProperty<bool> cursorVisibility;

        private void CursorVisibility_OnValueChanged(bool value) => Cursor.visible = value;
        private void CursorLockMode_OnValueChanged(CursorLockMode value) => Cursor.lockState = value;

        private void OnEnable()
        {
            CursorLockMode.OnValueChanged += CursorLockMode_OnValueChanged;
            CursorVisibility.OnValueChanged += CursorVisibility_OnValueChanged;
        }

        private void OnDisable()
        {
            CursorLockMode.OnValueChanged -= CursorLockMode_OnValueChanged;
            CursorVisibility.OnValueChanged -= CursorVisibility_OnValueChanged;
        }

        protected override void OnExistingInstanceFound(CursorManager existingInstance)
        {
            Destroy(gameObject);
        }
    }
}