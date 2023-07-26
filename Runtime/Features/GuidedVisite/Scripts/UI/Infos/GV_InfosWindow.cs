using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Michsky.MUIP;
using System;
using System.Reflection;

namespace Realit.Core.Features.GuidedVisite.UI
{
    public class GV_InfosWindow : FeatureComponent<GuidedVisite>
    {
        [SerializeField]
        HorizontalSelector roomSelector;
        [SerializeField]
        GV_InfoFiller generalInfos;
        [SerializeField]
        GV_InfoFiller roomInfos;

        GV_Point currentPoint;
        
        public GV_Point CurrentPoint
        {
            get => currentPoint;
            set
            {
                currentPoint = value;
                roomInfos.SetupInfos(currentPoint.infos, Feature.infoRepresentations);
            }
        }

        
        public void OnNewItemSelected(int index)
        {
            for (int i = 0; i < Feature.points.Length; i++)
            {
                if (Feature.points[i].PointName == roomSelector.items[index].itemTitle)
                {
                    CurrentPoint = Feature.points[i];
                    return;
                }
            }
        }

        protected override void OnFeatureInitiate()
        {
            GV_Point[] points = Feature.points;

            roomSelector.items.Clear();
            for (int i = 0; i < points.Length; i++)
                roomSelector.CreateNewItem(points[i].PointName);

            if(roomSelector.gameObject.activeInHierarchy)
                roomSelector.UpdateUI();

            generalInfos.Setup();
            roomInfos.Setup();

            var sceneInfos = GameObject.FindAnyObjectByType<GV_Scene>(FindObjectsInactive.Exclude);
            if(sceneInfos != null)
            {
                generalInfos.gameObject.SetActive(true);
                generalInfos.SetupInfos(sceneInfos.infos, Feature.infoRepresentations);
            }
            else
            {
                generalInfos.gameObject.SetActive(false);
            }

            Feature.OnPointChanged += OnPointChanges;
        }
        
        private void OnPointChanges(GV_Point point)
        {
            if (!gameObject.activeInHierarchy)
                return;

            foreach(var item in roomSelector.items)
            {
                if (point.PointName == item.itemTitle)
                {
                    int index = roomSelector.items.IndexOf(item);
                    roomSelector.index = index;
                    roomSelector.UpdateUI();
                    roomSelector.onValueChanged?.Invoke(index);
                    return;
                }
            }
        }

        protected override void OnFeatureStarts()
        {
            OnPointChanges(Feature.CurrentPoint);
        }

        protected override void OnFeatureEnds()
        {

        }
    }
}
