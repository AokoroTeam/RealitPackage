using System;
using System.Collections;
using System.Collections.Generic;

using LTX;

using Realit.Core.Managers;

using UnityEngine;
using UnityEngine.InputSystem;

namespace Realit.Core.VisiteSelection
{
    public class VisiteSelectionManager : Singleton<VisiteSelectionManager>
    {
        [SerializeField]
        private InputAction checkSelectionInput;
        [SerializeField]
        private InputAction hoverSelectionInput;
        [SerializeField]
        private LayerMask selectableLayer;

        public VisiteDestination selectedVisiteDestination;
        public static event Action<VisiteDestination> OnVisiteDestinationSelected;
        public static event Action<VisiteDestination> OnVisiteDestinationHovered;

        protected override void Awake()
        {
            checkSelectionInput.performed += CheckSelection_performed;
            hoverSelectionInput.performed += HoverSelectionInput_performed; ;
            base.Awake();
        }

        private void Start()
        {
            OnVisiteDestinationSelected?.Invoke(null);
            OnVisiteDestinationHovered?.Invoke(null);
        }
        private void OnEnable()
        {
            checkSelectionInput.Enable();
            hoverSelectionInput.Enable();
        }

        private void OnDisable()
        {
            checkSelectionInput.Disable();
            hoverSelectionInput.Disable();
        }

        private void CheckSelection_performed(InputAction.CallbackContext ctx)
        {
            Vector2 pointerScreenPos = Pointer.current.position.value;
            if(RealitUtilities.IsPointerOverUi(pointerScreenPos))
            {
                return;
            }

            var hitted = Physics.RaycastAll(Camera.main.ScreenPointToRay(pointerScreenPos), float.PositiveInfinity, selectableLayer);
            for (int i = 0; i < hitted.Length; i++)
            {
                RaycastHit hit = hitted[i];
                if(hit.collider.TryGetComponent(out VisiteDestination visite))
                {
                    selectedVisiteDestination = visite;
                    OnVisiteDestinationSelected?.Invoke(visite);
                    return;
                }
            }

            selectedVisiteDestination = null;
            OnVisiteDestinationSelected.Invoke(null);
        }

        private void HoverSelectionInput_performed(InputAction.CallbackContext ctx)
        {
            Vector2 pointerScreenPos = ctx.ReadValue<Vector2>();
            var hitted = Physics.RaycastAll(Camera.main.ScreenPointToRay(pointerScreenPos), float.PositiveInfinity, selectableLayer);
            for (int i = 0; i < hitted.Length; i++)
            {
                RaycastHit hit = hitted[i];
                if (hit.collider.TryGetComponent(out VisiteDestination visite))
                {
                    OnVisiteDestinationHovered?.Invoke(visite);
                    return;
                }
            }

            OnVisiteDestinationHovered?.Invoke(null);
        }

        public void GoToCurrentSelectedScene()
        {
            if(selectedVisiteDestination != null)
                Realit.Instance.LoadScene(selectedVisiteDestination.Visite);
        }

        protected override void OnExistingInstanceFound(VisiteSelectionManager existingInstance)
        {
            Destroy(gameObject);
        }
    }
}
