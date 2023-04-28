using Aokoro.Editor;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Aokoro.Settings.Editor
{

    [CustomPropertyDrawer(typeof(SettingsCategory))]
    public class SettingsCategoryDrawer : PropertyDrawer
    {
        Dictionary<string, ReorderableList> _listsPerProp = new Dictionary<string, ReorderableList>();

        public override bool CanCacheInspectorGUI(SerializedProperty property)
        {
            return true;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty settingsProperty = property.FindPropertyRelative(SettingsCategory.sectionPropertyName);
            float height = GetReorderableList(settingsProperty).GetHeight();

            height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative(SettingsCategory.categoryLabelPropertyName) , true);
            height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative(SettingsCategory.iconPropertyName) , true);

            height += EditorGUIUtility.singleLineHeight;
            
            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty sectionsProperty = property.FindPropertyRelative(SettingsCategory.sectionPropertyName);
            SerializedProperty categoryNameProperty = property.FindPropertyRelative(SettingsCategory.categoryLabelPropertyName);
            SerializedProperty iconProperty = property.FindPropertyRelative(SettingsCategory.iconPropertyName);

            EditorGUI.BeginProperty(position, label, property);

            float singleLineHeight = EditorGUIUtility.singleLineHeight;
            float verticalSpacing  = EditorGUIUtility.standardVerticalSpacing;


            //Values
            ReorderableList list = GetReorderableList(sectionsProperty);

            //Rects
            var categoryNameRect = new Rect(
                position.x, 
                position.y, 
                position.width, 
                singleLineHeight);

            var iconRect = new Rect(
                position.x, 
                position.y + verticalSpacing + singleLineHeight, 
                position.width,
                singleLineHeight);

            var sectionsRect = new Rect(
                position.x,
                position.y + (verticalSpacing + singleLineHeight) * 2,
                position.width,
                list.GetHeight());

            //Draw
            EditorGUI.PropertyField(categoryNameRect, categoryNameProperty);
            EditorGUI.PropertyField(iconRect, iconProperty);
            list.DoList(sectionsRect);

            EditorGUI.EndProperty();
            
        }

        private ReorderableList GetReorderableList(SerializedProperty listProperty)
        {
            ReorderableList list;
            if (_listsPerProp == null)
                _listsPerProp = new Dictionary<string, ReorderableList>();

            if (_listsPerProp.TryGetValue(listProperty.propertyPath, out list))
                return list;
            
            list = new ReorderableList(listProperty.serializedObject, listProperty, true, false, true, true);
            _listsPerProp[listProperty.propertyPath] = list;

            ///Drawers
            list.drawHeaderCallback = (Rect rect) => EditorGUI.LabelField(rect, "Sections");
            list.elementHeightCallback = idx => ElementHeight(idx, list);
            list.drawElementCallback = (rect, index, isActive, isFocused) => DrawElement(rect, index, list);
            list.onAddCallback = (ReorderableList list) => OnAdd(list);

            return list;
        }


        private static void DrawElement(Rect rect, int index, ReorderableList list)
        {
            SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index);
            EditorGUI.indentLevel++;
            if (element.hasVisibleChildren)
            {
                EditorGUI.PropertyField(rect, element, true);
            }
            else
                EditorGUI.PropertyField(rect, element, new GUIContent());
            EditorGUI.indentLevel--;
        }

        private static float ElementHeight(int idx, ReorderableList list)
        {
            SerializedProperty elementProp = list.serializedProperty.GetArrayElementAtIndex(idx);
            return EditorGUI.GetPropertyHeight(elementProp);
        }
        private void OnAdd(ReorderableList list)
        {
            SerializedProperty serializedProperty = list.serializedProperty;

            serializedProperty.arraySize++;
            var element = serializedProperty.GetArrayElementAtIndex(serializedProperty.arraySize - 1);

            element.FindPropertyRelative(SettingsSection.SettingsName)?.ClearArray();

            serializedProperty.serializedObject.ApplyModifiedProperties();
        }
    }

}
