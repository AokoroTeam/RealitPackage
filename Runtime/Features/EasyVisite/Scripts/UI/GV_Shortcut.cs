using NaughtyAttributes;
using TMPro;
using UnityEngine;
namespace Realit.Core.Features.GuidedVisite
{
    public class GV_Shortcut : FeatureComponent<GuidedVisite>
    {
        [ReadOnly]
        private GV_Point associatedPoint;
        [ReadOnly]
        public bool isOutOfScreen;

        [SerializeField]
        private TextMeshProUGUI[] labels;
        public RectTransform RectTransform
        {
            get
            {
                if(_rectTransform == null)
                    _rectTransform = transform as RectTransform;

                return _rectTransform;
            }
        }

        public GV_Point AssociatedPoint
        {
            get => associatedPoint;
            set
            {
                associatedPoint = value;
                for (int i = 0; i < labels.Length; i++)
                    labels[i].text = value.PointName;
            }
        }

        private RectTransform _rectTransform;

        private CanvasGroup canvasGroup;

        protected override void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            base.Awake();
        }

        private void OnEnable()
        {
            FadeIn();
        }

        public void FadeIn()
        {
            isOutOfScreen = false;
            canvasGroup.alpha = 1.0f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        public void FadeOut()
        {
            isOutOfScreen = true;
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        public void OnClick()
        {
            Feature.GoToPoint(AssociatedPoint, false);
        }

        protected override void OnFeatureInitiate()
        {

        }

        protected override void OnFeatureStarts()
        {

        }

        protected override void OnFeatureEnds()
        {

        }
    }
}
