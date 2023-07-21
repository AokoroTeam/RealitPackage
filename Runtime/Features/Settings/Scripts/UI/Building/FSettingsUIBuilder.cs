using LTX.Settings;
using LTX.Settings.UI;
using Michsky.MUIP;
using UnityEngine;

namespace Realit.Core.Features.Settings.UI
{
    public class FSettingsUIBuilder : SettingsUIBuilder<FSettingsCategoryUI, FSettingsUIBuilder>
    {
        SettingsUI ui;


        private void Awake()
        {
            ui = GetComponent<SettingsUI>();
        }

        public override void BuildUI()
        {
            base.BuildUI();
            ui.windowManager.InitializeWindows();
        }

        protected override bool CanCreateCategoryUI(SettingsCategory category)
        {
            return category.Sections.Length != 0;
        }

        protected override FSettingsCategoryUI CreateCategoryUI(SettingsCategory category)
        {
            FSettingsCategoryUI fSettingsCategoryUI = base.CreateCategoryUI(category);

            ui.tabs.CreateNewItem(category.categoryName, category.icon);
            ui.tabs.UpdateUI();

            WindowManager.WindowItem window = new()
            {
                windowObject = fSettingsCategoryUI.gameObject,
                windowName = category.categoryName
            };

            ui.windowManager.windows.Add(window);
            
            return fSettingsCategoryUI;
        }

        protected override SettingsHandler GetOrCreateSettingsHandler()
        {
            if(FeaturesManager.TryGetFeature(out Settings settings))
                MainSettingsManager.SettingsHandler.AddCategories(false, settings.GetFeaturesSettingsCategory());
            
            return MainSettingsManager.SettingsHandler;
        }
    }
}
