using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Realit.Core.Features;


namespace Realit.Core.Features.Tutorials
{
    public class MobileTutorial : FeatureComponent<Tutorial>
    {

        [SerializeField]
        GameObject more;


        public void ShowMore()
        {
            more.SetActive(true);
        }

        public void ShowLess()
        {
            more.SetActive(false);
        }

        protected override void OnFeatureInitiate()
        {

        }

        protected override void OnFeatureStarts()
        {
            if (Feature.IsMobile)
            {
                ShowLess();
                gameObject.SetActive(true);
            }
            else
                gameObject.SetActive(false);

        }

        protected override void OnFeatureEnds()
        {

        }
    }
}