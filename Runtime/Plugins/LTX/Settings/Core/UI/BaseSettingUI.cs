using LTX.Settings.UI.Internal;
using NaughtyAttributes;
using UnityEngine;

namespace LTX.Settings.UI
{
    public abstract class BaseSettingUI<T> : BaseSettingUI
    {
        [ShowNativeProperty]
        protected abstract SettingType Type { get; }

        protected virtual void OnEnable()
        {
            if (!string.IsNullOrEmpty(SettingPointer.internalName))
                Internal_SyncUIWithSetting();
        }

        public ISetting<T> Setting => SettingPointer.handler.GetSetting<T>(SettingPointer.internalName);

        protected abstract bool IsDirty();

        public abstract T GetValueFromUI();
        public abstract void SetUIFromValue(ISetting<T> setting);

        internal override void Internal_SyncUIWithSetting() => SetUIFromValue(Setting);
        internal override bool Internal_SyncSettingWithUI() => IsDirty() && SettingPointer.handler.TrySetSettingValue(SettingPointer.internalName, GetValueFromUI());
    }

    public abstract class BaseSettingUI : MonoBehaviour
    {
        [ReadOnly, BoxGroup("Base")]
        public SettingPointer SettingPointer;
        [ReadOnly, BoxGroup("Base")]
        public SettingsUIBuilder Builder;

        internal abstract void Internal_SyncUIWithSetting();
        internal abstract bool Internal_SyncSettingWithUI();

        public virtual void ResetSetting()
        {
            Internal_SyncSettingWithUI();

            Internal_SyncUIWithSetting();
        }
    }
}
