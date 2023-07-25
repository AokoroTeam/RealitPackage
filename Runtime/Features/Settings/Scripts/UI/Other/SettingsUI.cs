using Michsky.MUIP;
using NaughtyAttributes;
using Realit.Core.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace Realit.Core.Features.Settings.UI
{
    public class SettingsUI : FeatureComponent<Settings>
    {
        Animator animator;

        [SerializeField, BoxGroup("Components")]
        public HorizontalSelector tabs;
        [SerializeField, BoxGroup("Components")]
        public Transform bottomArea;
        [SerializeField, BoxGroup("Components")]
        public WindowManager windowManager;

        [SerializeField]
        private float baseWidth = 600;
        [SerializeField]
        private Canvas canvas;
        [SerializeField]
        RectTransform settingsSafeArea;
        [SerializeField]
        FSettingsUIBuilder uIBuilder;

        RectTransform rectTransform;

        public Canvas Canvas 
        { 
            get
            {
                if(canvas == null)
                {
                    canvas = GetComponentInParent<Canvas>();
                }

                return canvas;
            } 
        }

        protected override void Awake()
        {
            base.Awake();
            animator = GetComponent<Animator>();
            
            rectTransform = transform as RectTransform;
        }

        private void Update()
        {
            if (!Feature.IsActive)
                return;

            var safeArea = Screen.safeArea;

            float rightOffset = Canvas.pixelRect.xMax - safeArea.xMax;
            settingsSafeArea.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, rightOffset, baseWidth);
            rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0, baseWidth + rightOffset);
            RealitSceneManager.UI.borderOffsets.Write(this, new Vector4(0, -rectTransform.rect.width + rectTransform.anchoredPosition.x));
        }

        private void LateUpdate()
        {
            if (!Feature.IsActive)
                return;

            uIBuilder.ApplyDirtySettings();
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

        protected override void OnFeatureInitiate() 
        {
            RealitSceneManager.UI.borderOffsets.AddChannel(this, LTX.ChanneledProperties.PriorityTags.None);
        }

        protected override void OnFeatureStarts() 
        {
            animator.SetTrigger("In");
            //windowManager.OpenWindowByIndex(0);
            RealitSceneManager.UI.borderOffsets.ChangeChannelPriority(this, LTX.ChanneledProperties.PriorityTags.Highest);

            InputSystemUIInputModule inputModule = (EventSystem.current.currentInputModule as InputSystemUIInputModule);

            inputModule.rightClick.action.performed += Action_performed;
            inputModule.middleClick.action.performed += Action_performed;
            inputModule.leftClick.action.performed += Action_performed;

            windowManager.OpenWindow("Général");
            int idx = tabs.items.FindIndex(ctx => "Général" == ctx.itemTitle);
            tabs.index = idx;
            tabs.UpdateUI();
        }

        protected override void OnFeatureEnds() 
        {
            animator.SetTrigger("Out");
            RealitSceneManager.UI.borderOffsets.ChangeChannelPriority(this, LTX.ChanneledProperties.PriorityTags.None);

            InputSystemUIInputModule inputModule = (EventSystem.current.currentInputModule as InputSystemUIInputModule);

            inputModule.rightClick.action.performed -= Action_performed;
            inputModule.middleClick.action.performed -= Action_performed;
            inputModule.leftClick.action.performed -= Action_performed;
        }


        private void Action_performed(InputAction.CallbackContext ctx)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.9f)
                return;
            //Debug.Log(ctx.phase);
            if (!ctx.performed)
                return;
            
            Vector2 pos = Pointer.current.position.ReadValue();
            if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, pos))
                return;

            if (FeaturesManager.UI.TryGetUIForFeature(Feature, out var indicator))
            {
                RectTransform rt = indicator.transform as RectTransform;
                if (RectTransformUtility.RectangleContainsScreenPoint(rt, pos))
                    return;
            }

            Feature.EndFeature();
        }

    }
}
