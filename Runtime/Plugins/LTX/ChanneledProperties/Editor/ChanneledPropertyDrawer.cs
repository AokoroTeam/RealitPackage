#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using UnityEditorInternal;
using System.Reflection;
using UnityEngine.UIElements;

namespace LTX.ChanneledProperties.Editor
{

    
    [CustomPropertyDrawer(typeof(ChanneledProperty<>))]
    public class ChanneledPropertyDrawer : PropertyDrawer
    {
        private const string ChannelsPropertyName = "channels";
        private const string SlotsPropertyName = "availableSlots";

        private const string HasMainChannelPropertyName = "_hasMainChannel";
        private const string DefaultValuePropertyName = "_defaultValue";
        private const string CapacityPropertyName = "_capacity";
        private const string ExpandOnFullCapacityReachedPropertyName = "_expandOnFullCapacityReached";

        private bool channelFoldout;
        private bool infosFoldout;

        ReorderableList channelsList;

        SerializedProperty channelsProperty;
        SerializedProperty slotsProperty;
        SerializedProperty hasMainChannelProperty;
        SerializedProperty defaultValueProperty;
        SerializedProperty capacityProperty;
        SerializedProperty expandOnFullCapacityReachedProperty;



        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            //Label
            float height = EditorGUIUtility.singleLineHeight;

            height += GetChannelsHeight();
            height += GetInfosRectHeight();

            return height;
        }


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            float singleLineHeight = EditorGUIUtility.singleLineHeight;
            GetProperties(property);
            EditorGUI.BeginProperty(position, label, property);

            //Label
            var labelRect = new Rect(position.x, position.y, position.width, singleLineHeight);
            EditorGUI.LabelField(labelRect, label);

            position.x += 15;
            position.width -= 15;

            var channelsRect = new Rect(position.x, labelRect.yMax, position.width, singleLineHeight);
            float channelsHeight = GetChannelsHeight();

            DrawChannelFoldout(channelsRect, singleLineHeight);

            var infos = new Rect(position.x, channelsRect.y + channelsHeight, position.width, GetInfosRectHeight());
            DrawInfosFoldout(infos, singleLineHeight);

            EditorGUI.EndProperty();
            property?.serializedObject?.ApplyModifiedProperties();
        }

        private void GetProperties(SerializedProperty property)
        {
            channelsProperty = property.FindPropertyRelative(ChannelsPropertyName);
            slotsProperty = property.FindPropertyRelative(SlotsPropertyName);
            hasMainChannelProperty = property.FindPropertyRelative(HasMainChannelPropertyName);
            defaultValueProperty = property.FindPropertyRelative(DefaultValuePropertyName);
            capacityProperty = property.FindPropertyRelative(CapacityPropertyName);
            expandOnFullCapacityReachedProperty = property.FindPropertyRelative(ExpandOnFullCapacityReachedPropertyName);
        }

        private float GetChannelsHeight()
        {
            float height = EditorGUIUtility.singleLineHeight;

            if (channelFoldout)
            {
                if (channelsList.count > 0)
                    height += channelsList.GetHeight();
                else
                    height += EditorGUIUtility.singleLineHeight;
            }

            return height;
        }
        private float GetInfosRectHeight()
        {
            float singleLineHeight = EditorGUIUtility.singleLineHeight;
            float height = singleLineHeight;

            if (infosFoldout)
            {
                if (hasMainChannelProperty != null)
                    height += singleLineHeight;

                if (defaultValueProperty != null)
                    height += singleLineHeight;

                if (capacityProperty != null)
                    height += singleLineHeight;

                if (expandOnFullCapacityReachedProperty != null)
                    height += singleLineHeight;
            }

            return height;
        }
        private void DrawInfosFoldout(Rect totalRect, float singleLineHeight)
        {
            infosFoldout = EditorGUI.Foldout(totalRect, infosFoldout, "Infos", EditorStyles.foldoutHeader);
            

            if (infosFoldout)
            {
                float yOffset = singleLineHeight;
                GUI.enabled = false;

                if (hasMainChannelProperty != null)
                    DrawSingleInfo(hasMainChannelProperty);

                if (defaultValueProperty != null)
                    DrawSingleInfo(defaultValueProperty);
                
                if (capacityProperty != null)

                    DrawSingleInfo(capacityProperty);
            
                if (expandOnFullCapacityReachedProperty != null)
                    DrawSingleInfo(expandOnFullCapacityReachedProperty);

                GUI.enabled = true;

                void DrawSingleInfo(SerializedProperty property)
                {
                    Rect rect = new Rect(totalRect);
                    rect.y += yOffset;
                    rect.height = singleLineHeight;
                    EditorGUI.PropertyField(rect, property);

                    yOffset += singleLineHeight;
                }
            }
        }

        private void DrawChannelFoldout(Rect totalRect, float singleLineHeight)
        {
            float x = totalRect.x;
            float y = totalRect.y;
            float width = totalRect.width;
            float height = totalRect.height;

            var foldoutRect = new Rect(x, y, width, singleLineHeight);
            channelFoldout = EditorGUI.Foldout(foldoutRect, channelFoldout, "Channels", EditorStyles.foldoutHeader);

            if (channelFoldout)
            {
                //Values
                channelsList = GetChannelList(channelsProperty, slotsProperty);

                if (channelsList.count > 0)
                {
                    float minHeight = Mathf.Min(channelsList.GetHeight(), height);
                    var rect = new Rect(x, y + singleLineHeight, width, minHeight);
                    channelsList.DoList(rect);
                }
                else
                {
                    var rect = new Rect(x + 15, y + singleLineHeight, width - 20, singleLineHeight);
                    EditorGUI.LabelField(rect, "No channels yet", EditorStyles.helpBox);
                }
            }
        }

        private ReorderableList GetChannelList(SerializedProperty channels, SerializedProperty slotsProperty)
        {
            ReorderableList list;

            List<SerializedProperty> displayList = new List<SerializedProperty>();

            for (int i = 0; i < channels.arraySize; ++i)
            {
                if (slotsProperty.GetArrayElementAtIndex(i).boolValue)
                    continue;

                SerializedProperty item = channels.GetArrayElementAtIndex(i);
                
                displayList.Add(item);
            }
            
            //Now we had a display list and the list add and remove not affect source SerializedProperty.
            list = new ReorderableList(displayList, Type.GetType(channels.arrayElementType), false, false, false, false);

            list.elementHeightCallback = idx => ElementHeight(idx, list, slotsProperty);
            list.drawElementCallback = (rect, index, isActive, isFocused) => DrawChannelElement(rect, index, list, slotsProperty);

            return list;
        }


        private static void DrawChannelElement(Rect rect, int index, ReorderableList list, SerializedProperty slotsProperty)
        {
            if (!slotsProperty.GetArrayElementAtIndex(index).boolValue)
            {
                GUI.enabled = false;
                SerializedProperty element = list.list[index] as SerializedProperty;
                EditorGUI.indentLevel++;
                EditorGUI.PropertyField(rect, element, true);
                EditorGUI.indentLevel--;
                GUI.enabled = true;
            }
        }

        private static float ElementHeight(int index, ReorderableList list, SerializedProperty slotsProperty)
        {
            if (!slotsProperty.GetArrayElementAtIndex(index).boolValue)
            {
                SerializedProperty elementProp = list.list[index] as SerializedProperty;
                return EditorGUI.GetPropertyHeight(elementProp);
            }
            else
                return 0;
        }

        private void EnsureValidState()
        {

        }
    }
}
#endif