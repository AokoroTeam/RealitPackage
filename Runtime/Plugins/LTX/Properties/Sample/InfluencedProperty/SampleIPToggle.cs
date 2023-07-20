using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace LTX.ChanneledProperties.Samples
{
    public class SampleIPToggle : MonoBehaviour
    {
        [SerializeField]
        TextMeshProUGUI text;
        [SerializeField]
        Toggle toggle;
        
        [SerializeField]
        int value;
        [SerializeField]
        Influence influence;
        [SerializeField]
        int layer;
        [SerializeField]
        int orderInLayer;

        ChannelKey channelKey;
        SampleIPManager manager;

        private void Awake()
        {
            toggle.isOn = true;

            channelKey = ChannelKey.GetUniqueChannelKey();
            manager = GetComponentInParent<SampleIPManager>();
        }

        private void OnValidate()
        {
            text.SetText($"{influence} => {value}");
        }

        private void Start()
        {
            ChangeChannelActivity(toggle.isOn);
        }


        public void ChangeChannelActivity(bool isOn)
        {
            if(isOn)
                manager.calcul.AddChannel(channelKey, value, influence, layer, orderInLayer);
            else
                manager.calcul.RemoveChannel(channelKey);
        }
    }
}