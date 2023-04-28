using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.UI;
using TMPro;

namespace Realit.Core.Features.Settings.UI
{
    public class TabButton : MonoBehaviour
    {
        [SerializeField, BoxGroup("Data")]
        private string text;
        [SerializeField, BoxGroup("Data")]
        private Sprite icon;
        [SerializeField, BoxGroup("Objects")]
        private Image[] images;
        [SerializeField, BoxGroup("Objects")]
        private TextMeshProUGUI[] labels;

        private void OnValidate()
        {
            for (int i = 0; i < images.Length; i++)
                images[i].sprite = icon;

            for (int i = 0; i < labels.Length; i++)
                labels[i].text = text;
        }
    }
}
