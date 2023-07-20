
#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using UnityEditorInternal;

namespace LTX.ChanneledProperties.Editor
{

    
    [CustomPropertyDrawer(typeof(InfluencedProperty<>))]
    public class InfluencedPropertyDrawer : ChanneledPropertyDrawer
    {
        private const string StartValuePropertyName = "_startValue";
        private const string ValuePropertyName = "_value";

        SerializedProperty startValueProperty;
        SerializedProperty ValueProperty;


        protected override void GetProperties(SerializedProperty property)
        {
            base.GetProperties(property);

            startValueProperty = property.FindPropertyRelative(StartValuePropertyName);
            ValueProperty = property.FindPropertyRelative(ValuePropertyName);
        }
        

        protected override float GetInfosRectHeight()
        {
            var height = base.GetInfosRectHeight();
            float singleLineHeight = EditorGUIUtility.singleLineHeight;

            if (infosFoldout)
            {
                if (startValueProperty != null)
                    height += singleLineHeight;

                if (ValueProperty != null)
                    height += singleLineHeight;
            }

            return height;
        }

        protected override bool DrawInfosFoldout(Rect totalRect, float singleLineHeight, out float yOffset)
        {
            if(base.DrawInfosFoldout(totalRect, singleLineHeight, out yOffset))
            {
                Rect innerRect = new Rect(totalRect);
                innerRect.x += 20;
                innerRect.width -= 20;

                GUI.enabled = false;

                if (startValueProperty != null)
                    DrawSingleInfo(startValueProperty, innerRect, ref yOffset);

                if (ValueProperty != null)
                    DrawSingleInfo(ValueProperty, innerRect, ref yOffset);

                GUI.enabled = true;

                return true;
            }

            return false;
        }
    }
}
#endif