using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using NaughtyAttributes;

namespace Aokoro.UI
{
    [ExecuteInEditMode]
    public class TextAutoSizeController : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text[] TextObjects;

        Dictionary<TMP_Text, int> lastValues;
        private void Awake()
        {
            UpdateTextSize();
        }


        private void Update()
        {
            bool needRefresh = false;

            if(lastValues == null)
                lastValues = new Dictionary<TMP_Text, int>();

            if (TextObjects == null)
                TextObjects = new TMP_Text[0];
            for (int i = 0; i < TextObjects.Length; i++)
            {
                TMP_Text txt = TextObjects[i];
                int currentTxtHash = txt.text.GetHashCode();

                if (!lastValues.TryGetValue(txt, out int lastHash))
                {
                    needRefresh = true;
                    lastValues.Add(txt, currentTxtHash);
                }
                else if(lastHash != currentTxtHash)
                {
                    needRefresh = true;
                    lastValues[txt] = currentTxtHash;
                }
            }

            if (needRefresh)
                UpdateTextSize();
        }
        [Button]
        private void ScanForChildren()
        {
            TextObjects = GetComponentsInChildren<TMP_Text>();
        }
        [Button]
        private void UpdateTextSize()
        {
            if (TextObjects == null || TextObjects.Length == 0)
                return;

            // Iterate over each of the text objects in the array to find a good test candidate
            // There are different ways to figure out the best candidate
            // Preferred width works fine for single line text objects
            int candidateIndex = 0;
            float maxPreferred = 0;

            for (int i = 0; i < TextObjects.Length; i++)
            {
                float preferred = TextObjects[i].GetPreferredValues().sqrMagnitude;

                //Debug.Log($"{TextObjects[i].text} is {preferred}", TextObjects[i]);
                if (preferred > maxPreferred)
                {
                    maxPreferred = preferred;
                    candidateIndex = i;
                }
            }

            //Debug.Log($"Candidate is {TextObjects[candidateIndex].text}", TextObjects[candidateIndex]);

            // Force an update of the candidate text object so we can retrieve its optimum point size.
            TextObjects[candidateIndex].enableAutoSizing = true;
            TextObjects[candidateIndex].ForceMeshUpdate();
            float optimumPointSize = TextObjects[candidateIndex].fontSize;

            // Disable auto size on our test candidate
            TextObjects[candidateIndex].enableAutoSizing = false;

            // Iterate over all other text objects to set the point size
            for (int i = 0; i < TextObjects.Length; i++)
            {
                TextObjects[i].enableAutoSizing = false;
                TextObjects[i].fontSize = optimumPointSize;
            }
        }
    }
}
