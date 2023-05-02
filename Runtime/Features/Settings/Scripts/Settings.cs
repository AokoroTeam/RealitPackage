using Aokoro.Settings;
using LTX.ChanneledProperties;
using LTX.Settings;
using Realit.Core.Features.Settings.UI;
using Realit.Core.Managers;
using System.Collections.Generic;
using UnityEngine;

namespace Realit.Core.Features.Settings
{

    [System.Serializable]
    public class Settings : Feature
    {
        public Settings(FeatureDataAsset asset) : base(asset) { }

        public FSettingsUIBuilder uiBuilder;
        public SettingsHandler SettingsHandler => uiBuilder.SettingsHandler;



        public SettingsCategory GetFeaturesSettingsCategory()
        {
            Settings_Data _Data = Data as Settings_Data;

            var features = FeaturesManager.FeaturesList;
            List<SettingsSection> sections = new List<SettingsSection>();

            foreach (var feature in features)
            {
                if(feature.Data.hasSettings)
                    sections.AddRange(feature.Data.settings);
            }

            return new SettingsCategory("Features", _Data.tabIcon, sections.ToArray());
        }
        protected override void OnLoad()
        {
            Settings_Data _Data = Data as Settings_Data;
            MainSettingsManager.SettingProvider = new PlayerPrefsProvider();
            
            var windowItem = RealitSceneManager.UI.CreateWindow(_Data.windowName, _Data.window);
            uiBuilder = windowItem.windowObject.GetComponent<FSettingsUIBuilder>();

            //Channels
            RealitSceneManager.UI.windowPriority.AddChannel(this, PriorityTags.None, _Data.windowName);

            RealitSceneManager.Player.Freezed.AddChannel(this, PriorityTags.None, true);

            GameNotifications.Instance.canUpdate.AddChannel(this, PriorityTags.None, false);

            CursorManager.Instance.cursorLockMode.AddChannel(this, PriorityTags.None, CursorLockMode.Confined);
            CursorManager.Instance.cursorVisibility.AddChannel(this, PriorityTags.None, true);
        }
        protected override void OnUnload()
        {
            Settings_Data _Data = Data as Settings_Data;

            RealitSceneManager.UI.DestroyWindow(_Data.windowName);
            
            //Channels
            RealitSceneManager.UI.windowPriority.RemoveChannel(_Data.windowName);

            RealitSceneManager.Player.Freezed.RemoveChannel(this);

            GameNotifications.Instance.canUpdate.RemoveChannel(this);

            CursorManager.Instance.cursorLockMode.RemoveChannel(this);
            CursorManager.Instance.cursorVisibility.RemoveChannel(this);
        }

        protected override void OnStart()
        {
            Settings_Data _Data = Data as Settings_Data;

            RealitSceneManager.UI.windowPriority.ChangeChannelPriority(this, 500);

            RealitSceneManager.Player.Freezed.ChangeChannelPriority(this, PriorityTags.Highest);
            
            GameNotifications.Instance.canUpdate.ChangeChannelPriority(this, PriorityTags.Highest);
            
            CursorManager.Instance.cursorLockMode.ChangeChannelPriority(this, PriorityTags.Highest);
            CursorManager.Instance.cursorVisibility.ChangeChannelPriority(this, PriorityTags.Highest);
        }

        protected override void OnEnd()
        {
            Settings_Data _Data = Data as Settings_Data;

            if(uiBuilder != null)
                uiBuilder.WriteAllDirtySettings();

            RealitSceneManager.UI.windowPriority.ChangeChannelPriority(this, 0);

            RealitSceneManager.Player.Freezed.ChangeChannelPriority(this, PriorityTags.None);

            GameNotifications.Instance.canUpdate.ChangeChannelPriority(this, PriorityTags.None);
            
            CursorManager.Instance.cursorLockMode.ChangeChannelPriority(this, PriorityTags.None);
            CursorManager.Instance.cursorVisibility.ChangeChannelPriority(this, PriorityTags.None);
        }

        
    }
}