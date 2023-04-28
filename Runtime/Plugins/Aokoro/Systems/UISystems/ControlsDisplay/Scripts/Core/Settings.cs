using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Aokoro.UI.ControlsDiplaySystem
{
    [System.Serializable]
    public struct Settings
    {
        [SerializeField]
        private CD_ActionSettings[] settings;

        internal bool HasValue => settings != null;

        public Settings(params CD_ActionSettings[] selectors)
        {
            settings = selectors;
        }
        public Action[] ConvertInputSystemActions(IEnumerable<InputAction> actions)
        {
            List<Action> filteredActions = new List<Action>();
            List<InputAction> actionsList = new List<InputAction>(actions);

            for (int i = 0; i < settings.Length; i++)
            {
                var setting = settings[i];
                int indexOf = actionsList.FindIndex(ctx => ctx.name == setting.displayName);
                if (indexOf != -1)
                {
                    filteredActions.Add(new Action(setting, actionsList[indexOf]));
                    actionsList.RemoveAt(indexOf);
                }
            }

            return filteredActions.ToArray();
        }
    }
    [System.Serializable]
    public struct CD_ActionSettings
    {
        public string displayName;
        public string outputName;
        public List<int> bindings;


        public bool IsBindingRequested(int bindingIndex) => bindings.Count == 0 || bindings.Contains(bindingIndex);

        public CD_ActionSettings(string name)
        {
            this.displayName = name;
            this.outputName = name;
            bindings = new List<int>();
        }

        public CD_ActionSettings(string displayName, string outputName)
        {
            this.displayName = displayName;
            this.outputName = outputName;
            bindings = new List<int>();
        }
        public CD_ActionSettings(string displayName, string outputName, params int[] bindings)
        {
            this.displayName = displayName;
            this.outputName = outputName;
            this.bindings = new List<int>(bindings);
        }
    }
}