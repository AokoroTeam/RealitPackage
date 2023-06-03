using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;

namespace LTX.ControlsDisplay.UI
{
    public abstract class ControlIcon : MonoBehaviour
    {
        public abstract void SetupIcon(Control control);

    }
}
