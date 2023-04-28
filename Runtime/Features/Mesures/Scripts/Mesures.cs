using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Realit.Core.Features.Mesures
{

    [System.Serializable]
    public class Mesures : Feature
    {
        public Mesures(FeatureDataAsset asset) : base(asset)
        {
        }


        protected override void OnLoad()
        {

        }
        protected override void OnUnload()
        {

        }

        protected override void OnStart()
        {
            var rullers = GameObject.FindObjectsOfType<WorldRuller>(true);
            for (int i = 0; i < rullers.Length; i++)
            {
                rullers[i].Activate();
            }
        }

        protected override void OnEnd()
        {
            var rullers = GameObject.FindObjectsOfType<WorldRuller>(true);
            for (int i = 0; i < rullers.Length; i++)
            {
                rullers[i].Deactivate();
            }
        }
    }
}
