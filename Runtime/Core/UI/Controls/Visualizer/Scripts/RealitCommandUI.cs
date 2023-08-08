using LTX.ControlsVisualizer;
using LTX.ControlsVisualizer.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Realit.Core.Player.Controls
{
    [ExecuteInEditMode]
    public class RealitCommandUI : CommandUI
    {
        [SerializeField]
        TextMeshProUGUI label;

        protected override void ApplyCommandData(CommandUIContent data)
        {

        }
    }
}
