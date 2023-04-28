using Aokoro.Settings.UI.Internal;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Aokoro.Settings.UI
{
    public abstract class BaseSettingUI : MonoBehaviour
    {
        [ReadOnly, BoxGroup("Base")]
        public SettingPointer SettingPointer;
        [ReadOnly, BoxGroup("Base")]
        public SettingsUIBuilder Builder;

        [ShowNativeProperty]
        protected abstract SettingType Type { get; }


        protected ISetting Setting => SettingPointer.GetSetting();

        
        protected virtual void OnEnable()
        {
            if(!string.IsNullOrEmpty(SettingPointer.internalName))
                SyncUIWithSetting(Setting);
        }

        protected virtual void OnDisable()
        {

        }
        public virtual void ResetSetting()
        {
            var s = SyncSettingWithUI();
            s.Reset();

            SyncUIWithSetting(s);
        }
        public abstract void SyncUIWithSetting(ISetting setting);
        protected abstract ISetting SyncSettingWithUI();
        protected abstract bool IsDirty();

        internal void ApplyIfDirty()
        {
            if(IsDirty())
                SettingPointer.handler.ApplySetting(SyncSettingWithUI());
        }
    }
}
