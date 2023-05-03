#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using UnityEditorInternal;
using System.Reflection;

namespace LTX.ChanneledProperties.Editor
{
    /*
    [CustomPropertyDrawer(typeof(ChanneledProperty<>))]
    public class ChanneledPropertyDrawer : PropertyDrawer
    {
        private const string ChannelsPropertyName = "channels";
        private const string SlotsPropertyName = "availableSlots";

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = 0;
            SerializedProperty channelsProperty = property.FindPropertyRelative(ChannelsPropertyName);
            SerializedProperty slotsProperty = property.FindPropertyRelative(SlotsPropertyName);

            if (channelsProperty != null)
                height += GetChannelList(channelsProperty, slotsProperty).GetHeight();

            height += EditorGUIUtility.singleLineHeight;
            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty channelsProperty = property.FindPropertyRelative(ChannelsPropertyName);
            SerializedProperty slotsProperty = property.FindPropertyRelative(SlotsPropertyName);

            if (channelsProperty == null)
                return;

            EditorGUI.BeginProperty(position, label, property);

            //Values
            ReorderableList list = GetChannelList(channelsProperty, slotsProperty);

            if (list.count > 0)
            {
                var rect = new Rect(position.x, position.y, position.width, list.GetHeight());
                list.DoList(rect);
            }
            else
            {
                var rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
                EditorGUI.LabelField(rect, "No channels yet");
            }

            EditorGUI.EndProperty();

            list.serializedProperty.serializedObject.ApplyModifiedProperties();
        }

        private ReorderableList GetChannelList(SerializedProperty channels, SerializedProperty slotsProperty)
        {
            ReorderableList list;

            list = new ReorderableList(channels.serializedObject, channels, false, false, false, false);

            List<SerializedProperty> displayList = new List<SerializedProperty>();

            for (int i = 0; i < channels.arraySize; ++i)
            {
                if (slotsProperty.GetArrayElementAtIndex(i).boolValue)
                    continue;

                displayList.Add(channels.GetArrayElementAtIndex(i));
            }
            //Now we had a display list and the list add and remove not affect source SerializedProperty.
            list.list = displayList;
            list.elementHeightCallback = idx => ElementHeight(idx, list, slotsProperty);
            list.drawElementCallback = (rect, index, isActive, isFocused) => DrawChannelElement(rect, index, list, slotsProperty);

            return list;
        }


        private static void DrawChannelElement(Rect rect, int index, ReorderableList list, SerializedProperty slotsProperty)
        {
            if (!slotsProperty.GetArrayElementAtIndex(index).boolValue)
            {
                SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index);
                EditorGUI.indentLevel++;
                EditorGUI.PropertyField(rect, element, true);
                EditorGUI.indentLevel--;
            }
        }

        private static float ElementHeight(int idx, ReorderableList list, SerializedProperty slotsProperty)
        {
            if (!slotsProperty.GetArrayElementAtIndex(idx).boolValue)
            {
                SerializedProperty elementProp = list.serializedProperty.GetArrayElementAtIndex(idx);
                return EditorGUI.GetPropertyHeight(elementProp);
            }
            else
                return 0;
        }
    }
    */
}
#endif