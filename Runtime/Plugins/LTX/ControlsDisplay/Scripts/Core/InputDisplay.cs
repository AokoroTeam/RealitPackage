using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.InputSystem;
using System;

namespace LTX.ControlsDisplay
{
    [System.Serializable]
    public struct InputDisplay
    {
        public GameObject representation;
        [SerializeField] bool isComposite;
        [SerializeField] string[] matchPaths;

        public bool MatchesControl(Control control)
        {
            if (control.IsComposite == isComposite)
            {
                foreach (var controlData in control)
                {
                    string path = (isComposite ? control.compositeType : controlData.Path).Trim().ToLower();
                    for (int j = 0; j < matchPaths.Length; j++)
                    {
                        if (path == matchPaths[j])
                            return true;
                    }
                }
            }
            return false;
        }
        public static bool operator ==(InputDisplay c1, InputDisplay c2)
        {
            return c1.Equals(c2);
        }

        public static bool operator !=(InputDisplay c1, InputDisplay c2)
        {
            return !c1.Equals(c2);
        }

#if UNITY_EDITOR
        public void Validate()
        {
            for (int j = 0; j < matchPaths.Length; j++)
                matchPaths[j] = matchPaths[j].Trim().ToLower();
        }

        public override bool Equals(object obj)
        {
            return obj is InputDisplay input &&
                   EqualityComparer<GameObject>.Default.Equals(representation, input.representation) &&
                   EqualityComparer<string[]>.Default.Equals(matchPaths, input.matchPaths) ;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(representation, matchPaths);
        }
#endif
    }

    public class InputRepresentation
    {

        public readonly InputDisplay display;
        public readonly Control control;

        public InputRepresentation(InputDisplay display, Control control)
        {
            this.display = display;
            this.control = control;
        }
    }
}
