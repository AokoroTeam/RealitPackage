using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

namespace Aokoro.UI.ControlsDiplaySystem
{
    public class CommandDisplayer : MonoBehaviour
    {
        [SerializeField]
        TextMeshProUGUI description;
        [SerializeField]
        private Transform combinationsParents;
        [SerializeField]
        private GameObject combinationLayout;

        private CombinaisonDisplayer[] instantiatedCombinations;
        private List<GameObject> ands = new List<GameObject>();
        private List<GameObject> ors = new List<GameObject>();
        private bool or, and;

        public virtual void Fill(Command command, bool withOr = true, bool withAnd = true)
        {
            or = withOr;
            and = withAnd;

            description.text = command.actionName;

            Clear();
            instantiatedCombinations = new CombinaisonDisplayer[command.CombinationsCount];
            CreateCommandDisplays(command, instantiatedCombinations);
        }


        protected virtual void CreateCommandDisplays(Command command, CombinaisonDisplayer[] output)
        {
            for (int i = 0; i < command.CombinationsCount; i++)
            {
                if (i > 0)
                    Or(combinationsParents);

                InputCombination combination = command[i];
                output[i] = CreateCombinationDisplays(combination);
            }
        }

        protected virtual CombinaisonDisplayer CreateCombinationDisplays(InputCombination combination)
        {
            CombinaisonDisplayer display = Instantiate(combinationLayout, combinationsParents)
                .GetComponent<CombinaisonDisplayer>();

            display.Fill(combination, this);

            return display;
        }

        public virtual void And(Transform root)
        {
            if (and)
                ands.Add(Instantiate(ControlsDiplaySystem.Data.And, root));
        }

        public virtual void Or(Transform root)
        {
            if (or)
                ors.Add(Instantiate(ControlsDiplaySystem.Data.Or, root));
        }


        public void Clear()
        {
            if (instantiatedCombinations != null)
            {
                for (int i = 0; i < instantiatedCombinations.Length; i++)
                {

                    CombinaisonDisplayer cD_DisplayCombination = instantiatedCombinations[i];
                    if (cD_DisplayCombination)
                    {
                        cD_DisplayCombination.Clear();
                        Destroy(cD_DisplayCombination.gameObject);
                    }
                }

                instantiatedCombinations = null;
            }
            if (ors != null)
            {
                foreach (var or in ors)
                    Destroy(or);
                ors.Clear();
            }
            if (ands != null)
            {

                foreach (var and in ands)
                    Destroy(and);
                ands.Clear();
            }

        }

        private void OnDestroy()
        {
            Clear();
        }
    }
}
