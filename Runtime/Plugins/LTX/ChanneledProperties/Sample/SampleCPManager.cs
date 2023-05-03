using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace LTX.ChanneledProperties.Samples
{
    public class SampleCPManager : MonoBehaviour
    {
        public ChanneledProperty<string> texts;

        [SerializeField]
        private TextMeshProUGUI text;

        private void Awake()
        {
            texts = new ChanneledProperty<string>("No channels");
            texts.OnValueChanged += Texts_OnValueChanged;
        }

        private void Texts_OnValueChanged(string value)
        {
            text.SetText(value);
        }
    }
}
