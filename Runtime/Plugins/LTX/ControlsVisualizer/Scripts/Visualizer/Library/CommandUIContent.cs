using LTX.ControlsVisualizer.Abstraction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

namespace LTX.ControlsVisualizer.UI
{
    [System.Serializable]
    public struct CommandUIContent
    {
        public Command command;
        [SerializeField]
        private CommandUIData commandUIData;
        [SerializeField]
        private InputUIData[] inputUIDatas;

        public CommandUIContent(Command command) : this()
        {
            this.command = command;
            inputUIDatas = new InputUIData[command.Inputs.Length];
            //validCommands = new List<int>();
        }

        public void CollectCommandUI(CommandUIData[] commandUIs)
        {
            for (int i = 0; i < commandUIs.Length; i++)
            {
                CommandUIData commandUI = commandUIs[i];
                if(commandUI.commandType == command.Type)
                {

                    commandUIData = commandUI;
                    return;
                }
            }
        }

        public void CollectInputsUI(InputUIData[] inputsUI)
        {
            CommandInput[] inputs = command.Inputs;
            
            for (int i = 0; i < inputs.Length; i++)
            {
                CommandInput input = inputs[i];
                for (int j = 0; j < inputsUI.Length; j++)
                {
                    InputUIData inputVisual = inputsUI[j];

                    if (inputVisual.Matches(input.Path))
                    {
                        inputUIDatas[i] = inputVisual;

                        break;
                    }    
                }
            }
        }

        public InputUIData TryGetDataForInputIndex(int index) => inputUIDatas[index];

    }
}
