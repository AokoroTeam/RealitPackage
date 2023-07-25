using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Realit.Core.Features.GuidedVisite
{
    public class GV_InfoField : MonoBehaviour
    {
        [SerializeField]
        Image icon;
        [SerializeField]
        TextMeshProUGUI label;
        [SerializeField]
        TextMeshProUGUI data;

        public void SetData(Sprite icon, string label, string data)
        {
            this.icon.sprite = icon;
            this.label.text = label;
            this.data.text = data;
        }
    }
}
