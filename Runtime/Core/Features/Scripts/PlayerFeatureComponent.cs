using LTX.Entities.Player;

using Realit.Core.Player;
using Realit.Core.Player.Controls;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Realit.Core.Features
{

    public abstract class PlayerFeatureComponent<T> : FeatureComponent<T>, IPlayerFeatureComponent where T : Feature
    {
        public event System.Action OnActionsNeedRefresh;
        
        [SerializeField] 
        private string mapName;
        [SerializeField] 
        private InputActionAsset actions;

        public string MapName => mapName;
        public RealitPlayer Player { get; set; }
        public InputActionAsset ActionAsset { get => actions; set => actions = value; }

    }
}