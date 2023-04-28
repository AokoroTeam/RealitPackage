using Aokoro.Settings.UI.Internal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aokoro.Settings.UI
{
    public interface ISettingUIElement<T> where T : SettingsUIBuilder
    {
        public T UIBuilder { get; set; }
    }
}
