using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Realit.Core.VisiteSelection
{
    public class VisiteDestinationUI : MonoBehaviour
    {
        [SerializeField]
        CanvasGroup selectedGroup;
        [SerializeField]
        CanvasGroup nothingSelectedGroup;

        [SerializeField]
        private TextMeshProUGUI label;
        [SerializeField]
        private RawImage rawImage;


        private void OnEnable()
        {
            VisiteSelectionManager.OnVisiteDestinationSelected += VisiteSelectionManager_OnVisiteDestinationSelected;
        }


        private void OnDisable()
        {
            VisiteSelectionManager.OnVisiteDestinationSelected -= VisiteSelectionManager_OnVisiteDestinationSelected;   
        }

        private void VisiteSelectionManager_OnVisiteDestinationSelected(VisiteDestination ctx)
        {
            if(ctx == null)
            {
                nothingSelectedGroup.alpha = 1;
                selectedGroup.alpha = 0;
                selectedGroup.interactable = false;
            }
            else
            {
                nothingSelectedGroup.alpha = 0;
                selectedGroup.alpha = 1;
                selectedGroup.interactable = true;

                label.SetText(ctx.Visite.SceneDisplayName);
                rawImage.texture = ctx.Visite.ScenePreview;
            }
        }
    }
}
