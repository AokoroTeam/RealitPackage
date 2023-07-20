#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using UnityEditorInternal;

namespace LTX.ChanneledProperties.Editor
{

    
    [CustomPropertyDrawer(typeof(PrioritisedProperty<>))]
    public class PrioritisedPriorityDrawer : ChanneledPropertyDrawer
    {
        private const string HasMainChannelPropertyName = "_hasMainChannel";
        private const string DefaultValuePropertyName = "_defaultValue";

        SerializedProperty hasMainChannelProperty;
        SerializedProperty defaultValueProperty;


        protected override void GetProperties(SerializedProperty property)
        {
            base.GetProperties(property);

            hasMainChannelProperty = property.FindPropertyRelative(HasMainChannelPropertyName);
            defaultValueProperty = property.FindPropertyRelative(DefaultValuePropertyName);
        }
        

        protected override float GetInfosRectHeight()
        {
            var height = base.GetInfosRectHeight();
            float singleLineHeight = EditorGUIUtility.singleLineHeight;

            if (infosFoldout)
            {
                if (hasMainChannelProperty != null)
                    height += singleLineHeight;

                if (defaultValueProperty != null)
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

                if (hasMainChannelProperty != null)
                    DrawSingleInfo(hasMainChannelProperty, innerRect, ref yOffset);

                if (defaultValueProperty != null)
                    DrawSingleInfo(defaultValueProperty, innerRect, ref yOffset);

                GUI.enabled = true;

                return true;
            }

            return false;
        }
    }
}
#endif