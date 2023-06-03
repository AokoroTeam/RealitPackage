using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LTX.ControlsVisualizer.Abstraction
{
    [System.Serializable]
    public struct Command
    {
        [SerializeField]
        private Input[] inputs;
        
        public CommandType keyword;

        public int Lenght => inputs.Length;
        public Input this[int i]
        {
            get
            {
                return inputs[i];
            }
        }

    }

    public enum CommandType
    {
        And,
        Or,
        Then,
    }
}
