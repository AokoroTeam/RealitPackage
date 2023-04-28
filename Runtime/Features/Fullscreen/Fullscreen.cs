using Realit.Core.Features.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Screen = UnityEngine.Device.Screen;
using Application = UnityEngine.Device.Application;
using SystemInfo = UnityEngine.Device.SystemInfo;

#if UNITY_WEBGL
using static MarksAssets.FullscreenWebGL.FullscreenWebGL;
#endif

namespace Realit.Core.Features.Fullscreen
{
    [System.Serializable]
    public class Fullscreen : Feature
    {

        public event Action OnFullScreenEnter;
        public event Action OnFullScreenExit;

        public static bool IsFullScreenSupported =>
#if UNITY_WEBGL 
        isFullscreenSupported() && !Application.isEditor;
#else
        false;
#endif

        public static bool IsFullScreen =>
#if UNITY_WEBGL
            isFullscreen();
#else
            Screen.fullScreen;
#endif

        public Fullscreen(FeatureDataAsset asset) : base(asset)
        {

        }

        public void RequestFullScreen()
        {
            Debug.Log("Requesting fullscreen");
#if UNITY_WEBGL
            requestFullscreen(navUI: navigationUI.hide);
            if (Application.isMobilePlatform)
                Screen.orientation = ScreenOrientation.LandscapeLeft;
#else
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
            Screen.fullScreen = true;      
            OnFullScreenChange();
#endif
        }

        public void DisableFullScreen()
        {
            Debug.Log("Exiting fullscreen");
#if UNITY_WEBGL
            exitFullscreen();

#else
            Screen.fullScreen = false;
            OnFullScreenChange();
#endif
        }

        private void OnFullScreenChange()
        {
            if (IsFullScreen)
            {
                OnFullScreenEnter?.Invoke();
                if (FeaturesManager.UI.TryGetUIForFeature(this, out FeatureIndicator icon))
                    icon.SetIcon((Data as Fullscreen_Data).shrink);
            }
            else
            {
                OnFullScreenExit?.Invoke();
                if (FeaturesManager.UI.TryGetUIForFeature(this, out FeatureIndicator icon))
                    icon.SetIcon((Data as Fullscreen_Data).expand);
            }
        }

        protected override void OnStart()
        {
            if(!IsFullScreen)
                RequestFullScreen();
            
        }

        protected override void OnEnd()
        {
            if (IsFullScreen)
                DisableFullScreen();
        }


        protected override void OnLoad()
        {
#if UNITY_WEBGL
            subscribeToFullscreenchangedEvent();
            onfullscreenchange += OnFullScreenChange;
#endif
        }
        protected override void OnUnload()
        {
#if UNITY_WEBGL
            unsubscribeToFullscreenchangedEvent();
            onfullscreenchange -= OnFullScreenChange;
#endif
        }
    }
}