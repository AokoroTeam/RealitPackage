using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LTX.ControlsVisualizer.Abstraction
{
    public enum CommandType
    {
        None,
        Input,
        And,
        Axis,
    }

    /// <summary>
    /// Input combinaision.
    /// 
    /// A control is composed with one or multiple commands.
    /// A command can have sub-command called children if it is a composite command.
    /// </summary>
    [System.Serializable]
    public struct Command
    {
        public static char DeviceLayoutsSeparator = '&';
        public readonly bool IsComposite => _inputs.Length > 1;
        public readonly CommandInput[] Inputs => _inputs;
        public readonly CommandType Type => IsComposite ? _type : CommandType.Input;
        public readonly string[] DeviceLayouts => _deviceLayout.Split(DeviceLayoutsSeparator);


        [SerializeField]
        private CommandInput[] _inputs;
        [SerializeField]
        private CommandType _type;
        [SerializeField]
        private string _deviceLayout;

        
        public Command(string deviceLayout, CommandInput input) : this(deviceLayout, CommandType.Input, input)
        {

        }

        public Command(string deviceLayout, CommandType type, params CommandInput[] inputs) : this()
        {
            _type = type;
            _deviceLayout = deviceLayout;

            
            for (int i = 0; i < inputs.Length; i++)
                inputs[i]._deviceLayout = deviceLayout;
            _inputs = inputs;
        }
    }
}
