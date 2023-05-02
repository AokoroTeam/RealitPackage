using LTX.UI;
using System.Collections.Generic;
using UnityEngine;

namespace Realit.Core.Features.GuidedVisite.UI
{
    public class GV_NearbyPoints : UIItem
    {
        [SerializeField]
        GameObject shortcutPrefab;

        GuidedVisite easyVisite;
        Dictionary<GV_Point, GV_Shortcut> shortcuts = new Dictionary<GV_Point, GV_Shortcut>();


        private Camera MainCamera
        {
            get
            {
                if(_mainCam == null)
                    _mainCam = Camera.main;
                return _mainCam;
            }
        }

        private Camera _mainCam;

        public GuidedVisite EasyVisite
        {
            get => easyVisite;
            set
            {
                if(value != easyVisite)
                {
                    //New
                    if(value != null)
                        value.OnPointChanged += OnPointChanged;
                    //Old
                    if(easyVisite != null)
                        easyVisite.OnPointChanged -= OnPointChanged;
                }

                easyVisite = value;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            if(FeaturesManager.AreFeaturesLoaded)
            {
                if (FeaturesManager.TryGetFeature(out GuidedVisite EV))
                    OnLoaded(EV);
                else
                    gameObject.SetActive(false);
            }
            else
                FeaturesManager.AddFeatureLoadedCallbackListener<GuidedVisite>(OnLoaded);
        }

        private void OnLoaded(Feature f)
        {
            EasyVisite = f as GuidedVisite;
            FeaturesManager.RemoveFeatureLoadedCallbackListener<GuidedVisite>(OnLoaded);

            GenerateAllShorcuts();
        }

        protected override void OnUpdate()
        {
            if(EasyVisite != null && shortcuts != null)
            {
                foreach (var s in shortcuts)
                {
                    GV_Shortcut shortcut = s.Value;
                    GV_Point point = s.Key;

                    if (shortcut.gameObject.activeInHierarchy)
                    {
                        Vector2 pointScreenPosition = MainCamera.WorldToScreenPoint(point.transform.position);
                        bool outOfBounds = !Screen.safeArea.Contains(pointScreenPosition) || !IsVisibleFrom(point.transform, MainCamera);

                       

                        if (outOfBounds)
                        {
                            if(!shortcut.isOutOfScreen)
                                shortcut.FadeOut();
                        }
                        else
                        {
                            if(shortcut.isOutOfScreen)
                                shortcut.FadeIn();

                            shortcut.RectTransform.position = pointScreenPosition;
                        }
                    }
                }
            }
        }
        private bool IsVisibleFrom(Transform t, Camera camera)
        {
            Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
            return GeometryUtility.TestPlanesAABB(planes, new Bounds(t.position, Vector3.one * .01f));
        }

        private void OnPointChanged(GV_Point point) => ActivateNearbyPoints();

        private void GenerateAllShorcuts()
        {
            if (shortcuts != null)
            {
                foreach (var s in shortcuts)
                    Destroy(s.Value.gameObject);

                shortcuts.Clear();
            }
            else
                shortcuts = new Dictionary<GV_Point, GV_Shortcut>();

            var points = easyVisite.points;

            for (int i = 0; i < points.Length; i++)
            {
                var shortcut = Instantiate(shortcutPrefab, transform).GetComponent<GV_Shortcut>();
                shortcut.AssociatedPoint = points[i];
                shortcut.gameObject.SetActive(false);

                shortcuts.Add(points[i], shortcut);
            }
        }


        private void ActivateNearbyPoints()
        {
            if (easyVisite != null)
            {
                GV_Point[] points = easyVisite.points;

                for (int i = 0; i < points.Length; i++)
                {
                    GV_Point p = points[i];
                    if(shortcuts.TryGetValue(p, out GV_Shortcut s))
                    {
                        s.gameObject.SetActive(
                            easyVisite.CurrentPoint != null && 
                            easyVisite.CurrentPoint.nearbyPoints.Contains(p));
                    }
                }
            }
            else
            {
                foreach (var s in shortcuts)
                    s.Value.gameObject.SetActive(false);
            }
        }
    }
}
