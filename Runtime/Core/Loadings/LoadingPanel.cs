using LTX.ChanneledProperties;
using Michsky.MUIP;
using TMPro;
using UnityEngine;

namespace Realit.Core.UI
{
    public class LoadingPanel : MonoBehaviour
    {
        ChanneledProperty<(float, string)> progresses;

        [SerializeField]
        private ProgressBar bar;
        [SerializeField]
        private TextMeshProUGUI tmp_label;

        private CanvasGroup canvasGroup;

        
        private void Awake()
        {
            progresses = new ChanneledProperty<(float, string)>((0, "..."));
            canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = 1;
            gameObject.SetActive(false);
        }
        private void OnEnable()
        {
            bar.currentPercent = 0;
            bar.UpdateUI();
        }

        private void LateUpdate()
        {
            if (progresses.ChannelCount == 0)
                gameObject.SetActive(false);
        }



        public void StartLoadingScreen(Object key, int priority)
        {
            progresses.AddChannel(key, priority);
            gameObject.SetActive(true);
        }

        public void StopLoadingScreen(Object key)
        {
            progresses.RemoveChannel(key);
            if (progresses.ChannelCount == 0)
                gameObject.SetActive(false);
        }

        public void UpdateBar(Object key, float progress, string label)
        {
            if(progresses.HasChannel(key))
                progresses.Write(key, (progress, label));


            (float, string) value = progresses.Value;

            bar.currentPercent = value.Item1;
            tmp_label.SetText(value.Item2);
            
            bar.UpdateUI();
            //Canvas.ForceUpdateCanvases();
        }

    }
}