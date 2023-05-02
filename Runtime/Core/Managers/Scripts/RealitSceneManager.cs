using Aokoro.UI;
using LTX;

using Realit.Core.Features;
using Realit.Core.Player;
using System;
using UnityEngine;

namespace Realit.Core.Managers
{


    [DefaultExecutionOrder(-80)]
    [AddComponentMenu("Realit/Reader/Managers/Scene manager")]
    public class RealitSceneManager : Singleton<RealitSceneManager>
    {
        public static UIManager UI { get; private set; }

        public enum InitialisationPhase
        {
            None,
            Level,
            Player,
            Features,
            Entities,
            Others,
            Done,
        }

        /// Initialisation order : 
        /// Level
        /// Player
        /// Features
        /// Entities
        /// Others
        /// 
        public InitialisationPhase LevelInitiationPhase { get; private set; } = InitialisationPhase.None;


        public static event Action<Realit_Player> OnPlayerIsSetup;

        
        public static Realit_Player Player { get; private set; }


        public void InitializeScene(FeatureDataAsset[] features)
        {
            Debug.Log($"[Scene Manager] Initializing Scene");
            
            InitializeLevel();
            InitializePlayer();
            InitializeFeatures(features);
            InitializeOthers();

            LevelInitiationPhase = InitialisationPhase.Done;

            Debug.Log($"[Scene Manager] Scene initialized");
        }


        protected void InitializeLevel()
        {
            LevelInitiationPhase = InitialisationPhase.Level;
            UI = GameObject.FindGameObjectWithTag("MainUI").GetComponent<UIManager>();
        }


        protected void InitializePlayer()
        {
            LevelInitiationPhase = InitialisationPhase.Player;
            Player = Realit_Player.LocalPlayer as Realit_Player;
            
            Player.OnAwake();

            OnPlayerIsSetup?.Invoke(Player);
        }

        protected void InitializeFeatures(FeatureDataAsset[] loadedFeatures)
        {
            LevelInitiationPhase = InitialisationPhase.Features;
            FeaturesManager.LoadFeatures(loadedFeatures);
        }

        protected void InitializeOthers()
        {
            LevelInitiationPhase = InitialisationPhase.Others;

        }

        protected override void OnExistingInstanceFound(RealitSceneManager existingInstance)
        {
            Destroy(gameObject);
        }

        private void OnDisable()
        {
            //Debug.Log("Setting event to null");
            OnPlayerIsSetup = null;
        }
    }
}
