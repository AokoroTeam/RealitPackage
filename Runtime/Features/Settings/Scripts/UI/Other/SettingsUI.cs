using Michsky.MUIP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Realit.Core.Features.Settings
{
    public class SettingsUI : FeatureComponent<Settings>
    {
        Animator animator;

        [SerializeField]
        public HorizontalSelector tabs;
        [SerializeField]
        public Transform bottomArea;
        [SerializeField]
        public WindowManager windowManager;

        protected override void Awake()
        {
            base.Awake();
            animator = GetComponent<Animator>();
        }

        public void OpenCategory(int index)
        {
            string windowName = tabs.items[index].itemTitle;
            windowManager.OpenWindow(windowName);
        }

        public void CloseWindow()
        {
            FeaturesManager.EndFeature(Feature.Data.FeatureName);
        }

        public void GoToMainMenu()
        {
            Realit.Instance.LoadMainMenu();
        }

        public void ReloadScene()
        {
            Realit.Instance.LoadScene(Realit.Instance.sceneProfile);
        }

        protected override void OnFeatureInitiate() { }
        protected override void OnFeatureStarts() 
        {
            animator.SetTrigger("In");
        }
        protected override void OnFeatureEnds() 
        {
            animator.SetTrigger("Out");
        }
    }
}
