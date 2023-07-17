using LTX.ChanneledProperties;
using NaughtyAttributes;

using System;

using UnityEngine;
using UnityEngine.InputSystem;

namespace LTX.Entities.Player
{
    public class PlayerManager : Entity
    {
        public event Action<string, string> OnMapChange;
        public event Action OnRespawn;
        public event Action OnLocalPlayerDisabled;
        public event Action OnLocalPlayerEnabled;

        [HideInInspector]
        public PlayerInput playerInput;

        [HideInInspector]
        public Animator anim;
        [HideInInspector]
        public Rigidbody rb;
        //[Space]
        //[ReadOnly, BoxGroup("Inputs")]
        //public InputActionMap currentMap;
        private AudioListener audioListener;
  
        public AudioListener AudioListener
        {
            get
            {
                if (audioListener == null)
                    audioListener = Camera.main.GetComponent<AudioListener>();

                return audioListener;
            }
        }

        public static PlayerManager LocalPlayer
        {
            get
            {
                if (localPlayer == null)
                {
                    GameObject player = GameObject.FindGameObjectWithTag("Player");
                    if (player != null)
                        localPlayer = player.GetComponent<PlayerManager>();

                }

                return localPlayer;
            }
        }
        private static PlayerManager localPlayer;

        public static GameObject PlayerContainer;

        protected override void Awake()
        {
            if (LocalPlayer != this)
            {
                Destroy(gameObject);
                return;
            }

            playerInput = GetComponent<PlayerInput>();
            
            anim = GetComponent<Animator>();
            rb = GetComponent<Rigidbody>();
;
            
            SetupVariables();
        }
        public virtual void OnAwake()
        {
            Initiate<PlayerManager>();
        }

        protected override void Initiate<T>()
        {
            SetupCursorForPlayer();

            PlayerContainer = new GameObject("Player container");
            transform.SetParent(PlayerContainer.transform);
            base.Initiate<T>();
        }

        protected virtual void SetupCursorForPlayer()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public void Respawn(Vector3 position, Quaternion rotation)
        {
            rb.position = position;
            rb.rotation = rotation;
            OnRespawn?.Invoke();
        }
        private void OnEnable()
        {
            if(LocalPlayer == this)
                OnLocalPlayerEnabled?.Invoke();
        }
        private void OnDisable()
        {
            if(LocalPlayer == this)
            {
                OnLocalPlayerDisabled?.Invoke();
                localPlayer = null;
            }
        }
    }
}
