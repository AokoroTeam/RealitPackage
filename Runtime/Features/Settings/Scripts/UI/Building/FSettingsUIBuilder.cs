using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Aokoro.Settings.UI;
using Aokoro.Settings;
using Michsky.MUIP;

namespace Realit.Core.Features.Settings.UI
{
    public class FSettingsUIBuilder : SettingsUIBuilder<FSettingsCategoryUI, FSettingsUIBuilder>
    {
        [SerializeField]
        private GameObject tabPrefab;
        [SerializeField]
        private Transform tabsParent;
        [SerializeField]
        public Transform bottomArea;


        private WindowManager windowManager;


        private void Awake()
        {
            windowManager = GetComponent<WindowManager>();
        }

        public override void BuildUI()
        {
            base.BuildUI();

            windowManager.windows.Find(ctx => ctx.windowName == "Autre")?.buttonObject.transform.SetAsLastSibling();
            windowManager.InitializeWindows();
        }

        protected override FSettingsCategoryUI CreateCategoryUI(SettingsCategory category)
        {
            FSettingsCategoryUI fSettingsCategoryUI = base.CreateCategoryUI(category);

            var tabGO = Instantiate(tabPrefab, tabsParent);
            
            tabGO.transform.SetSiblingIndex(tabsParent.childCount - 1);
            
            ButtonManager buttonManager = tabGO.GetComponent<ButtonManager>();

            tabGO.name = $"Tab_{category.categoryName}";
            buttonManager.SetText(category.categoryName);
            buttonManager.SetIcon(category.icon);

            WindowManager.WindowItem window = new()
            {
                buttonObject = tabGO,
                windowObject = fSettingsCategoryUI.gameObject,
                windowName = category.categoryName,
            };

            windowManager.windows.Add(window);
            
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
