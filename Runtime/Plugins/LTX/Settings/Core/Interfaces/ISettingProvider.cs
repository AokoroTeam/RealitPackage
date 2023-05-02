using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LTX.Settings
{
    public interface ISettingProvider
    {
        public bool TryReadSetting(ref ISetting setting);
        public bool TryWriteSetting(ref ISetting setting);
    }
}
