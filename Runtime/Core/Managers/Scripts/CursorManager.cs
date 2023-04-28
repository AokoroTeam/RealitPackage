using Aokoro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Realit.Core.Managers
{
    public class CursorManager : Singleton<CursorManager>
    {
        public ChanneledProperty<CursorLockMode> cursorLockMode = new ChanneledProperty<CursorLockMode>();
        public ChanneledProperty<bool> cursorVisibility = new ChanneledProperty<bool>();


        private void CursorVisibility_OnValueChanged(bool value) => Cursor.visible = value;
        private void CursorLockMode_OnValueChanged(CursorLockMode value) => Cursor.lockState = value;

        private void OnEnable()
        {
            cursorLockMode.OnValueChanged += CursorLockMode_OnValueChanged;
            cursorVisibility.OnValueChanged += CursorVisibility_OnValueChanged;
        }

        private void OnDisable()
        {
            cursorLockMode.OnValueChanged -= CursorLockMode_OnValueChanged;
            cursorVisibility.OnValueChanged -= CursorVisibility_OnValueChanged;
        }

        protected override void OnExistingInstanceFound(CursorManager existingInstance)
        {
            Destroy(gameObject);
        }
    }
}