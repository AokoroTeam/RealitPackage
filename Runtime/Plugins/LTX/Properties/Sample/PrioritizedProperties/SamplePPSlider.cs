using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace LTX.ChanneledProperties.Samples
{
    public class SamplePPSlider : MonoBehaviour
    {
        [SerializeField]
        TextMeshProUGUI text;
        [SerializeField]
        Slider slider;
        [SerializeField]
        Toggle toggle;
        [SerializeField]
        string value;

        ChannelKey channelKey;
        SamplePPManager manager;

        private void Awake()
        {
            slider.wholeNumbers = true;

            slider.minValue = 0;
            slider.maxValue = 10;

            toggle.isOn = true;

            channelKey = ChannelKey.GetUniqueChannelKey();
            manager = GetComponentInParent<SamplePPManager>();
        }

        private void OnValidate()
        {
            text.SetText(value);
        }

        private void Start()
        {
            ChangeChannelActivity(toggle.isOn);
            ChangeSliderValue(slider.value);
        }

        public void ChangeSliderValue(float value)
        {
            manager.texts.ChangeChannelPriority(channelKey, (int)value);
        }

        public void ChangeChannelActivity(bool isOn)
        {
            if(isOn)
                manager.texts.AddChannel(channelKey, (int)slider.value, text.text);
            else
                manager.texts.RemoveChannel(channelKey);
        }
    }
}