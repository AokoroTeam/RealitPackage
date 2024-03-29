using NaughtyAttributes;
using UnityEngine;

namespace LTX.UI
{
    public abstract class UIItem : MonoBehaviour
    {
        protected BaseUIManager uIManager;
        protected Canvas canvas => uIManager.canvas;

        [SerializeField, BoxGroup("UI item")]
        bool hasUpdate = true;

        protected virtual void Awake()
        {
            uIManager = GetComponentInParent<BaseUIManager>();
            if (uIManager == null)
                Destroy(this);
        }

        protected virtual void OnEnable()
        {
            if (hasUpdate)
            {
                if (uIManager != null)
                    uIManager.OnUpdate += OnUpdate;
            }
        }
        protected virtual void OnDisable()
        {
            if (hasUpdate)
            {
                if (uIManager != null)
                    uIManager.OnUpdate -= OnUpdate;
            }
        }

        protected abstract void OnUpdate();
    }
}