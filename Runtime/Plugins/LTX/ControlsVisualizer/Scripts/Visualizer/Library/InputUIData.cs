using LTX.ControlsVisualizer.Abstraction;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using UnityEngine;

namespace LTX.ControlsVisualizer.UI
{

    [System.Serializable]
    public struct InputUIData
    {
        [SerializeField] private string[] paths;
        [SerializeField] private GameObject prefab;

        public bool HasInputUI => prefab != null;
        public GameObject Prefab => prefab;
        
        internal bool Matches(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return false;

            for (int i = 0; i < paths.Length; i++)
            {
                string subject = paths[i];

                string regexPattern = string.Concat("^", Regex.Escape(path).Replace("\\*", ".*"), "$");

                int wildcardCount = path.Count(x => x.Equals('*'));
                bool success = false;

                if (wildcardCount <= 0)
                    success = subject.Equals(path, StringComparison.CurrentCultureIgnoreCase);
                
                else if (wildcardCount == 1)
                {
                    string newWildcardPattern = path.Replace("*", "");

                    if (path.StartsWith("*"))
                        success = subject.EndsWith(newWildcardPattern, StringComparison.CurrentCultureIgnoreCase);
                    else if (path.EndsWith("*"))
                        success = subject.StartsWith(newWildcardPattern, StringComparison.CurrentCultureIgnoreCase);
                    else
                        success = Regex.IsMatch(subject, regexPattern);
                }
                else
                    success = Regex.IsMatch(subject, regexPattern);

                if (success)
                    return true;
            }

            return false;
        }
    }
}
