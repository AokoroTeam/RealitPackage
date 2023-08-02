using LTX.Settings;
using NaughtyAttributes;
using Realit.Core.Managers;
using Realit.Core.Player;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Realit.Core.Player.Controls
{
    public class MobileControls : MonoBehaviour
    {
        private const string SchemeSettingName = "MobileControlScheme";        
        public string TargetSchemeName
        {
            get
            {
                var ts = defaultScheme.SchemeName;

                if (MainSettingsManager.TryGetSettingValue(SchemeSettingName, out int scheme))
                {
                    foreach (var kv in schemes)
                    {
                        if (kv.Value == scheme)
                        {
                            ts = kv.Key.SchemeName;
                            break;
                        }
                    }
                }
                return ts;
            }
        }

        [SerializeField, ReadOnly]
        private MobileControlScheme currentScheme;
        [SerializeField, Required]
        private MobileControlScheme defaultScheme;

        private Dictionary<MobileControlScheme, int> schemes;
        private CanvasGroup canvasGroup;
        private PlayerControls playerControls;

        private bool isMobile = false;

        private void Awake()
        {
            schemes = new Dictionary<MobileControlScheme, int>();

            var childrenSchemes = GetComponentsInChildren<MobileControlScheme>();

            MainSettingsManager.SettingsHandler.AddSettingChangeCallback(SchemeSettingName, SyncWithPlayerSetting);
            if (MainSettingsManager.SettingsHandler.TryGetSetting(SchemeSettingName, out ChoiceSetting choiceSetting))
            {
                string[] choices = choiceSetting.Choices;

                for (int i = 0; i < childrenSchemes.Length; i++)
                {
                    var mbs = childrenSchemes[i];
                    int index = -1;

                    for (int j = 0; j < choices.Length; j++)
                    {
                        if (choices[j] == mbs.SchemeName)
                        {
                            index = j;
                            break;
                        }
                    }

                    if (index != -1)
                        schemes.Add(mbs, index);
                    else
                    {
                        //Carrement
                        Destroy(mbs.gameObject);
                    }
                }
            }

            canvasGroup = GetComponent<CanvasGroup>();

            //EnhancedTouchSupport.Enable();

            if (RealitSceneManager.Player != null)
                SubscribeAndRefresh(RealitSceneManager.Player);

            RealitSceneManager.OnPlayerIsSetup += SubscribeAndRefresh;
        }

        private void OnDestroy()
        {
            //EnhancedTouchSupport.Disable();
            if (playerControls != null)
                playerControls.OnControlChanges -= SyncWithPlayerControlScheme;


            MainSettingsManager.SettingsHandler.RemoveSettingChangeCallback(SchemeSettingName, SyncWithPlayerSetting);
            RealitSceneManager.OnPlayerIsSetup -= SubscribeAndRefresh;
        }

        void Update()
        {
            if(currentScheme != null)
                currentScheme.PerformControls();
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

        private void SubscribeAndRefresh(Realit_Player playerManager)
        {
            if (playerManager.GetLivingComponent(out playerControls))
            {
                playerControls.OnControlChanges += SyncWithPlayerControlScheme;
                SyncWithPlayerControlScheme(playerControls);
            }
        }

        public void ActivateMobileControls(string schemeName, bool writeIntoSetting = true)
        {
            if (playerControls == null)
                return;

            gameObject.SetActive(true);
            isMobile = true;

            currentScheme = null;

            foreach (var scheme in schemes)
            {
                if (scheme.Key.SchemeName == schemeName)
                {
                    currentScheme = scheme.Key;
                    currentScheme.EnableScheme(playerControls);

                    if (writeIntoSetting)
                        MainSettingsManager.TrySetSettingValue(SchemeSettingName, scheme.Value);
                }
                else
                    scheme.Key.DisableScheme(playerControls);
            }
        }

        private void DeactiveMobileControls()
        {
            currentScheme = null;
            gameObject.SetActive(false);
            isMobile = false;

            foreach (var scheme in schemes)
                scheme.Key.DisableScheme(playerControls);
        }

        private void SyncWithPlayerControlScheme(PlayerControls controls)
        {
            string playerControlScheme = controls.PlayerInput.currentControlScheme;
            bool isMobile = playerControlScheme == "Mobile";

            //No sync necessary
            if (isMobile == this.isMobile)
                return;

            if(isMobile)
                ActivateMobileControls(TargetSchemeName);
            else
                DeactiveMobileControls();
        }

        private void SyncWithPlayerSetting(ISetting setting)
        {
            if(setting is ChoiceSetting choiceSetting)
                ActivateMobileControls(choiceSetting.GetChoice(choiceSetting.Value), false);
        }

        public interface IMobileControl
        {
            public void Perform();
            public void Enable(PlayerControls player);
            public void Disable(PlayerControls player);
        }
    }
}
