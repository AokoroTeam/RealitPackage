using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

using Screen = UnityEngine.Device.Screen;
using Application = UnityEngine.Device.Application;
using SystemInfo = UnityEngine.Device.SystemInfo;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using Realit.Core.Managers;
using Realit.Core.Features.UI;
using Realit.Core.Player;
using NaughtyAttributes;
using Aokoro.Settings;

namespace Realit.Core.Controls
{
    public class MobileControls : MonoBehaviour
    {
        private const string SchemeSettingName = "MobileControlScheme";
        
        [ShowNativeProperty]
        public string CurrentSchemeName => currentScheme == null ? string.Empty : currentScheme.SchemeName;

        [SerializeField, ReadOnly]
        private MobileControlScheme currentScheme;
        [SerializeField, Required]
        private MobileControlScheme defaultScheme;

        private MobileControlScheme[] schemes;
        private CanvasGroup canvasGroup;
        private Realit_Player player;

        private void Awake()
        {
            schemes = GetComponentsInChildren<MobileControlScheme>();
            canvasGroup = GetComponent<CanvasGroup>();

            //EnhancedTouchSupport.Enable();

            if (RealitSceneManager.Player != null)
                SubscribeAndRefresh(RealitSceneManager.Player);

            RealitSceneManager.OnPlayerIsSetup += SubscribeAndRefresh;
            SetDefaultScheme();
        }

        private void SetDefaultScheme()
        {
            if (MainSettingsManager.TryGetSettingValue(SchemeSettingName, out string scheme))
            {
                for (int i = 0; i < schemes.Length; i++)
                {
                    if (schemes[i].SchemeName == scheme)
                    {
                        currentScheme = schemes[i];
                        return;
                    }
                }
            }

            currentScheme = defaultScheme;
        }

        private void OnDestroy()
        {
            //EnhancedTouchSupport.Disable();

            if (RealitSceneManager.Player != null)
                RealitSceneManager.Player.OnControlChanges -= SyncWithPlayer;
            
            RealitSceneManager.OnPlayerIsSetup -= SubscribeAndRefresh;
        }





        // Update is called once per frame
        void Update()
        {
            if(currentScheme != null)
            {
                currentScheme.PerformControls();
            }
        }



        static List<RaycastResult> raycastResultsList = new List<RaycastResult>();
        public static bool IsPointerOverUi(Vector2 point)
        {
            PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = point;

            raycastResultsList.Clear();

            EventSystem.current.RaycastAll(pointerEventData, raycastResultsList);
            
            return raycastResultsList.Count > 0;
        }

        private void SubscribeAndRefresh(Realit_Player player)
        {
            this.player = player;
            player.OnControlChanges += SyncWithPlayer;
            SyncWithPlayer(player);
        }

        public void ChangeScheme(string schemeName)
        {
            if (player == null)
                return;

            for (int i = 0; i < schemes.Length; i++)
            {
                schemes[i].DisableScheme(player);

                if (schemes[i].SchemeName == schemeName)
                {
                    currentScheme = schemes[i];
                    break;
                }
            }

            if(currentScheme != null)
                currentScheme.EnableScheme(player);
        }

        private void SyncWithPlayer(Realit_Player realit_Player)
        {
            if (gameObject.activeInHierarchy)
            {
                string currentControlScheme = RealitSceneManager.Player.playerInput.currentControlScheme;

                bool isMobile = currentControlScheme == "Mobile";

                canvasGroup.alpha = isMobile ? 1 : 0;
                canvasGroup.interactable = isMobile;
                canvasGroup.blocksRaycasts = true;

                for (int i = 0; i < schemes.Length; i++)
                {
                    if (isMobile && schemes[i] == currentScheme)
                        schemes[i].EnableScheme(realit_Player);
                    else
                        schemes[i].DisableScheme(realit_Player);
                }
            }
        }

        public interface IMobileControl
        {
            public void Perform();
            public void Enable(Realit_Player player);
            public void Disable(Realit_Player player);
        }
    }
}
