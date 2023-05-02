using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LTX.Settings.Samples
{

    /// <summary>
    /// No saving whatsoever
    /// </summary>
    public class SampleSettingsProvider : ISettingProvider
    {

        public bool TryReadSetting(ref ISetting setting)
        {
            return true;
        }

        public bool TryWriteSetting(ref ISetting setting)
        {
            return true;
        }

    }
}
