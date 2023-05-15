using LTX.Settings;
using NaughtyAttributes;
using Realit.Core.Managers;
using Realit.Core.Player;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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
            ChangeScheme(string.Empty);
        }

        private void SyncSchemeWithSetting()
        {
            if (MainSettingsManager.TryGetSettingValue(SchemeSettingName, out string scheme))
                ChangeScheme(scheme, false);
            else
                ChangeScheme(defaultScheme.SchemeName);
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

        public void ChangeScheme(string schemeName, bool writeIntoSetting = true)
        {
            if (player == null)
                return;

            currentScheme = null;
            for (int i = 0; i < schemes.Length; i++)
            {
                if (schemes[i].SchemeName == schemeName)
                {
                    currentScheme = schemes[i];
                    currentScheme.EnableScheme(player);
                }
                else
                    schemes[i].DisableScheme(player);
            }

            if(writeIntoSetting)
                MainSettingsManager.TrySetSettingValue(SchemeSettingName, CurrentSchemeName);
        }

        private void SyncWithPlayer(Realit_Player realit_Player)
        {
            if (gameObject.activeInHierarchy)
            {
                string currentControlScheme = RealitSceneManager.Player.playerInput.currentControlScheme;

                bool isMobile = currentControlScheme == "Mobile";
                if(isMobile)
                    Debug.Log("[Mobile Controles] Activating touch control");
                else
                    Debug.Log("[Mobile Controles] Deactivating touch control");

                canvasGroup.alpha = isMobile ? 1 : 0;
                canvasGroup.interactable = isMobile;
                canvasGroup.blocksRaycasts = true;

                ChangeScheme(isMobile ? defaultScheme.SchemeName : string.Empty);
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
