using TMPro;
using UnityEngine;

namespace LTX.ChanneledProperties.Samples
{
    public class SampleIPManager : MonoBehaviour
    {
        public InfluencedProperty<int> calcul;

        [SerializeField]
        private TextMeshProUGUI text;

        private void Awake()
        {
            calcul = new(1);

            calcul.AddOnValueChangeCallback(Texts_OnValueChanged, true);
        }

        private void Texts_OnValueChanged(int value)
        {
            text.SetText(value.ToString());
        }
    }
}
