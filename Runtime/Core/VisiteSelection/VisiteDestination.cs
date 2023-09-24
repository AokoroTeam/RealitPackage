using System.Collections;
using System.Collections.Generic;

using Realit.Core.Scenes;

using UnityEngine;

namespace Realit.Core.VisiteSelection
{
    public class VisiteDestination : MonoBehaviour
    {
        private Outline outline;

        [SerializeField]
        private RealitScene visite;
        [SerializeField]
        private int nonHoveredWidth;
        [SerializeField]
        private int hoveredWidth;
        [SerializeField]
        private Color nonSelectedColor;
        [SerializeField]
        private Color selectedColor;


        public RealitScene Visite { get => visite; }

        private void Awake()
        {
            outline = GetComponent<Outline>();
        }
        private void OnEnable()
        {

            VisiteSelectionManager.OnVisiteDestinationHovered += VisiteSelectionManager_OnVisiteDestinationHovered;
            VisiteSelectionManager.OnVisiteDestinationSelected += VisiteSelectionManager_OnVisiteDestinationSelected;
        }

        private void OnDisable()
        {

            VisiteSelectionManager.OnVisiteDestinationHovered -= VisiteSelectionManager_OnVisiteDestinationHovered;
            VisiteSelectionManager.OnVisiteDestinationSelected -= VisiteSelectionManager_OnVisiteDestinationSelected;
        }

        private void VisiteSelectionManager_OnVisiteDestinationSelected(VisiteDestination obj)
        {
            if (obj == this)
                outline.OutlineColor = selectedColor;
            else
                outline.OutlineColor = nonSelectedColor;
        }

        private void VisiteSelectionManager_OnVisiteDestinationHovered(VisiteDestination obj)
        {
            if (obj == this)
                outline.OutlineWidth = hoveredWidth;
            else
                outline.OutlineWidth = nonHoveredWidth;
        }
    }
}
