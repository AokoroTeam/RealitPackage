using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using LTX.Editor;

namespace LTX.Settings.Editor
{

    [CustomPropertyDrawer(typeof(SettingsSection), true)]
    public class SettingsSectionDrawer : PropertyDrawer
    {
        private struct SettingCreationParams
        {
            public SettingType type;
            public SerializedProperty property;
        }
        Dictionary<string, ReorderableList> _listsPerProp = new Dictionary<string, ReorderableList>();

        public override bool CanCacheInspectorGUI(SerializedProperty property)
        {
            return true;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {   
            float height = 0;
            SerializedProperty settingsProperty = property.FindPropertyRelative(SettingsSection.SettingsName);

            if(settingsProperty != null)
                height += GetReorderableList(settingsProperty, property).GetHeight();

            height += EditorGUIUtility.singleLineHeight;
            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty settingsProperty = property.FindPropertyRelative(SettingsSection.SettingsName);

            if (settingsProperty == null)
                return;

            EditorGUI.BeginProperty(position, label, property);

            
            //Values
            ReorderableList list = GetReorderableList(settingsProperty, property);

            var settingsRect = new Rect(position.x, position.y, position.width, list.GetHeight());

            list.DoList(settingsRect);

            EditorGUI.EndProperty();
            
            list.serializedProperty.serializedObject.ApplyModifiedProperties();
        }

        private ReorderableList GetReorderableList(SerializedProperty listProperty, SerializedProperty sectionProperty)
        {
            ReorderableList list;
            if (_listsPerProp == null)
                _listsPerProp = new Dictionary<string, ReorderableList>();

            if (_listsPerProp.TryGetValue(listProperty.propertyPath, out list))
            {
                return list;
            }
            
            list = new ReorderableList(listProperty.serializedObject, listProperty, true, true, true, true);
            _listsPerProp[listProperty.propertyPath] = list;

            ///Drawers
            list.drawHeaderCallback = (Rect rect) =>
            {
                SerializedProperty label = sectionProperty.FindPropertyRelative(nameof(SettingsSection.Label));
                float width = GUI.skin.textField.CalcSize(new GUIContent(label.stringValue)).x + 20;

                EditorGUI.PropertyField(new Rect(rect.x, rect.y, Mathf.Min(Mathf.Max(width, 50), rect.width), rect.height ), label, GUIContent.none);
            };
            //list.onAddCallback = (ReorderableList list) => Debug.Log("Added");
            
            list.elementHeightCallback = idx => ElementHeight(idx, list);

            list.drawElementCallback = (rect, index, isActive, isFocused) => DrawElement(rect, index, list);

            list.onAddDropdownCallback = (Rect buttonRect, ReorderableList list) => OnAddDrowpdown(sectionProperty);

            return list;
        }


        private static void DrawElement(Rect rect, int index, ReorderableList list)
        {
            SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index);
            EditorGUI.indentLevel++;
            if (element != null)
            {
                if (element.isExpanded)
                {
                    EditorGUI.PropertyField(rect, element, true);
                }
                else
                {
                    SerializedProperty value = element.FindPropertyRelative(nameof(ISetting<object>.Value));

                    if (value != null)
                    {
                        float width = 130;
                        Rect valueRect = new Rect(rect.width - width, rect.y, width, rect.height);
                        EditorGUI.PropertyField(valueRect, value, GUIContent.none);
                        EditorGUI.PropertyField(rect, element, false);
                    }
                    else
                    {
                        Debug.Log("Could'nt find value");
                    }
                }
            }
            EditorGUI.indentLevel--;
        }

        private static float ElementHeight(int idx, ReorderableList list)
        {
            SerializedProperty elementProp = list.serializedProperty.GetArrayElementAtIndex(idx);
            return EditorGUI.GetPropertyHeight(elementProp);
        }
        private void OnAddDrowpdown(SerializedProperty sectionProperty)
        {
            var menu = new GenericMenu();

            SettingType[] settingTypes = Enum.GetValues(typeof(SettingType)) as SettingType[];

            foreach (var s in settingTypes)
                menu.AddItem(new GUIContent(s.ToString()), false, clickHandler, new SettingCreationParams()
                {
                    type = s,
                    property = sectionProperty,
                });

            menu.ShowAsContext();
        }


        void clickHandler(object target)
        {
            var data = (SettingCreationParams)target;
            SettingsSection section = data.property.GetValue<SettingsSection>();
            
            switch (data.type)
            {
                case SettingType.Boolean:
                    section.AddBoolean();
                    break;
                case SettingType.Float:
                    section.AddFloat();
                    break;

                case SettingType.Integer:
                    section.AddInteger();
                    break;
                case SettingType.Text:
                    section.AddText();
                    break;
                case SettingType.Vector2:
                    section.AddVector2();
                    break;
                case SettingType.Vector3:
                    section.AddVector3();
                    break;

                case SettingType.Choice:
                    section.AddChoice();
                    break;
            }

            data.property.serializedObject.Update();
        }
    }

}
