using LTX;
using LTX.Sequencing;

using NaughtyAttributes;
using Realit.Core.Managers;
using Realit.Core.Scenes;

using Realit.Core.UI;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Realit.Core
{
    [DefaultExecutionOrder(-100)]
    [AddComponentMenu("Realit/Reader/RealitReader")]
    public class Realit : Singleton<Realit>
    {
        public event Action<bool> OnBuiltInModeLoading;
        public event Action OnStartsLoading;
        public event Action OnEndsLoading;

        [BoxGroup("Ressources")]
        [SerializeField] GameObject canvasManagerAsset;
        [BoxGroup("Ressources")]
        [SerializeField] GameObject sceneManagerAsset;
        [BoxGroup("Ressources")]
        [SerializeField] GameObject playerAsset;

        [HideInInspector]
        public GameObject PlayerRessource;
        [BoxGroup("Demo")]
        public bool IsBuiltInMode;
        [BoxGroup("Demo"), ShowIf(nameof(IsBuiltInMode))]
        public bool setupInThisSceneAtLaunch;
        [BoxGroup("Demo"), Expandable, ShowIf(nameof(setupInThisSceneAtLaunch)), Space]
        public RealitBuiltInSceneProfile sceneProfile;

        [BoxGroup("Runtime"), ReadOnly]
        public bool IsReadyForLoading = false;
        [BoxGroup("Runtime")]
        public LoadingPanel loadingPanel;

        private Sequencer createManagerSeq;

        protected override void Awake()
        {
            base.Awake();

            if (Instance == this)
            {
                transform.SetParent(null);
                DontDestroyOnLoad(gameObject);
                CreateSequences();
            }
        }

        private void CreateSequences()
        {
            SequencerBuilder builder = new();

            createManagerSeq = builder
                .Do(() =>
                {
                    OnStartsLoading?.Invoke();
                    Instantiate(canvasManagerAsset, transform.parent);
                    Instantiate(sceneManagerAsset, transform.parent);
                });

            builder.Clear();
        }

        private void Start()
        {
            if (Instance == this)
            {
                IsReadyForLoading = true;

                if (setupInThisSceneAtLaunch)
                {
                    StartCoroutine(nameof(ISetupSceneWithProfile), sceneProfile);
                }
            }
        }

        public void LoadScene(RealitBuiltInSceneProfile sceneProfile)
        {
            if (IsReadyForLoading && IsBuiltInMode)
            {
                StopAllCoroutines();
                StartCoroutine(ILoadSceneProfile(sceneProfile));
            }
        }

        public void LoadMainMenu()
        {
            if(IsReadyForLoading)
            {
                StartCoroutine(IInternalLoadScene(0));
            }
        }
        private IEnumerator IInternalLoadScene(int sceneIndex)
        {
            loadingPanel.StartLoadingScreen(this, 100);
            var operation = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Single);
            operation.allowSceneActivation = true;

            while (!operation.isDone)
            {
                loadingPanel.UpdateBar(this, operation.progress, "Chargement de la scene...");
                yield return null;
            }

            if (SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(sceneIndex)))
                Debug.LogError($"Couldn't set {gameObject.scene.name} scene as active");

            loadingPanel.StopLoadingScreen(this);

        }
        private IEnumerator ILoadSceneProfile(RealitBuiltInSceneProfile sceneProfile)
        {
            loadingPanel.StartLoadingScreen(this, 100);
            this.sceneProfile = sceneProfile;
            OnBuiltInModeLoading?.Invoke(true);
            Debug.Log("[Realit Reader] Starting in builtIn mode");

            //Loading scene
            int value = SceneUtility.GetBuildIndexByScenePath(sceneProfile.SceneName);
            if (value == -1)
            {
                Debug.LogError($"Couldn't find scene with path  {sceneProfile.SceneName}");
            }
            else
            {

                yield return StartCoroutine(nameof(IInternalLoadScene), value);

                yield return StartCoroutine(nameof(ISetupSceneWithProfile), sceneProfile);
            }
        }

        private IEnumerator ISetupSceneWithProfile(RealitBuiltInSceneProfile sceneProfile)
        {
            loadingPanel.StartLoadingScreen(this, 100);

            //Creating managers
            createManagerSeq.Play();
            yield return new WaitWhile(() =>
            {
                bool v = createManagerSeq.IsRunning();
                return v;
            });

            //Spawning player
            ;
            Transform spawn = GameObject.FindGameObjectWithTag("Respawn").transform;
            var go = Instantiate(playerAsset, spawn.transform.position, spawn.transform.rotation);
            OnPlayerInstantiated(go);

            RealitSceneManager.Instance.InitializeScene(sceneProfile.features);

            loadingPanel.StopLoadingScreen(this);
        }
#region Setup

        public static void OnModelLoaded(GameObject modelGO)
        {

        }

        public static void OnPlayerInstantiated(GameObject playerGO)
        {

        }
#endregion

        
        protected override void OnExistingInstanceFound(Realit existingInstance)
        {
            Destroy(gameObject);
        }


    }
}
