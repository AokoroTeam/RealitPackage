using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

using System.Collections;
using System.Collections.Generic;

using Michsky.MUIP;

using Realit.Core.Scenes;
using Realit.Core;

namespace Realit.Core
{
    public class RealitMainMenu : MonoBehaviour
    {
        [SerializeField, ReadOnly]
        private int currentScene;

        [SerializeField]
        private RawImage background;
        [SerializeField]
        private RawImage blurredBackground;

        [SerializeField]
        HorizontalSelector selector;
        [SerializeField]
        GameObject closeButton;

        RealitScene[] scenes;

        private bool loading;

        private void Awake()
        {
#if UNITY_WEBGL
            closeButton.SetActive(false);
#endif
            if (Realit.BuildContent != null)
            {
                scenes = Realit.BuildContent.Scenes;

                HorizontalSelector.Item[] items = new HorizontalSelector.Item[scenes.Length];

                for (int i = 0; i < scenes.Length; i++)
                {
                    HorizontalSelector.Item item = new()
                    {
                        itemTitle = scenes[i].SceneName
                    };

                    items[i] = item;
                }

                selector.items = new List<HorizontalSelector.Item>(items);
            }

            loading = false;
        }

        private void Start()
        {
            selector.index = 0;
            currentScene = -1;
        }
        private void Update()
        {
            if (currentScene != selector.index)
            {
                currentScene = selector.index;
                UpdateUI();
            }
        }

        private void UpdateUI()
        {
            selector.index = currentScene;
            background.texture = scenes[currentScene].ScenePreview;
            blurredBackground.texture = scenes[currentScene].ScenePreview;

            selector.UpdateUI();
        }

        public void LoadCurrentSelectedScene()
        {
            if (!loading)
            {
                Realit.Instance.LoadScene(scenes[currentScene]);
                loading = true;
            }
        }

        public void CloseApp()
        {
            Application.Quit();
        }
    }
}