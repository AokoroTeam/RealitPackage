using LTX.ControlsVisualizer.Abstraction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LTX.ControlsVisualizer.UI
{
    public abstract class InputUI : MonoBehaviour
    {
        public abstract void ApplyData(CommandInput input);
    }
}
