using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LTX.ControlsDisplay
{
    public struct Command : IEnumerable<InputCombination>
    {
        public string actionName;

        private List<InputCombination> combinations;
        public int CombinationsCount => combinations.Count;


        public InputCombination this[int i] => combinations[i];
        public Command(string actionName)
        {
            this.actionName = actionName;
            combinations = new List<InputCombination>();
        }

        
        public void Addcombination(params InputRepresentation[] inputs) => this.combinations.Add(inputs);
        public void Addcombination(int range, InputRepresentation[] inputs)
        {
            InputRepresentation[] shortenInputs = new InputRepresentation[range];
            for (int i = 0; i < range; i++)
                shortenInputs[i] = inputs[i];

            combinations.Add(shortenInputs);
        }

        IEnumerator<InputCombination> IEnumerable<InputCombination>.GetEnumerator() => combinations.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => (this as IEnumerable).GetEnumerator();


    }
    public struct InputCombination : IEnumerable<InputRepresentation>
    {
        private InputRepresentation[] inputs;
        public InputCombination(InputRepresentation[] matchedInputs)
        {
            inputs = matchedInputs;
        }
        public InputRepresentation this[int i] => inputs[i];
        public int Length => inputs != null ? inputs.Length : 0;

        IEnumerator<InputRepresentation> IEnumerable<InputRepresentation>.GetEnumerator() => inputs.GetEnumerator() as IEnumerator<InputRepresentation>;

        IEnumerator IEnumerable.GetEnumerator() => (this as IEnumerable).GetEnumerator();


        public static implicit operator InputCombination(InputRepresentation[] inputs) => new InputCombination(inputs);
    }
}
