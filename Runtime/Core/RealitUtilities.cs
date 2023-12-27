using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Realit.Core
{
    public static class RealitUtilities
    {
        private static List<RaycastResult> raycastResultsList;

        public static List<RaycastResult> RaycastResultsList { get => raycastResultsList??=new(); }

        public static bool IsPointerOverUi(Vector2 point)
        {
            PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = point;

            RaycastResultsList.Clear();

            EventSystem.current.RaycastAll(pointerEventData, RaycastResultsList);

            return RaycastResultsList.Count > 0;
        }
    }
}
