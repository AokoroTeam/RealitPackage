using UnityEngine.InputSystem;

namespace LTX.ControlsDisplay
{
    public readonly struct Action
    {
        public readonly CD_ActionSettings settings;
        public readonly InputAction action;

        public Action(CD_ActionSettings settings, InputAction action)
        {
            this.settings = settings;
            this.action = action;
        }
        public Action(string name, InputAction action)
        {
            this.settings = new CD_ActionSettings(name);
            this.action = action;
        }
        public static implicit operator InputAction(Action _InputActions) => _InputActions.action;
    }
}
