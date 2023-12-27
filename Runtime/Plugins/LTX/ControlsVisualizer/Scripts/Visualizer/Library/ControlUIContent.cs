using LTX.ControlsVisualizer.Abstraction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LTX.ControlsVisualizer.UI
{
    [System.Serializable]
    public struct ControlUIContent
    {
        [SerializeField]
        public Control control;
        [SerializeField]
        public CommandUIContent[] commandsContent;

        public ControlUIContent(Control control)
        {
            this.control = control;
            int length = control.commands.Length;

            commandsContent = new CommandUIContent[length];
            
            for (int i = 0; i < length; i++)
            {
                CommandUIContent commandUIContent = new(control.commands[i]);
                commandsContent[i] = commandUIContent;
            }
        }

        public void CollectCommandsContent(CommandUIData[] commandUIs, DeviceUILibrary[] deviceUILibraries)
        {
            for (int i = 0; i < commandsContent.Length; i++)
            {
                CommandUIContent commandUIContent = commandsContent[i];
                commandUIContent.CollectCommandUI(commandUIs);

                for (int j = 0; j < deviceUILibraries.Length; j++)
                {
                    var lib = deviceUILibraries[j];
                    commandUIContent.CollectInputsUI(lib.inputUIs);
                }
            }
        }
    }
}
