using LTX.ControlsVisualizer.Abstraction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LTX.ControlsVisualizer.UI
{
    [System.Serializable]
    public struct CommandUIData
    {
        [SerializeField]
        internal CommandType commandType;
        [SerializeField]
        internal string[] layoutCompatibility;
        [SerializeField]
        internal GameObject commandUIPrefab;

        internal bool MatchesCommand(Command command) => command.Type == commandType;
    }
}
