using TMPro;
using UnityEngine;

namespace LTX.ChanneledProperties.Samples
{
    public class SamplePPManager : MonoBehaviour
    {
        public PrioritisedProperty<string> texts;

        [SerializeField]
        private TextMeshProUGUI text;

        private void Awake()
        {
            texts = new PrioritisedProperty<string>("No channels");

            texts.AddOnValueChangeCallback(Texts_OnValueChanged, true);
        }

        private void Texts_OnValueChanged(string value)
        {
            text.SetText(value);
        }
    }
}
