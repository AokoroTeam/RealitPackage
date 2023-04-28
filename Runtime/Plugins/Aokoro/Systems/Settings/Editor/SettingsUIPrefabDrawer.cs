using Aokoro.Settings.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Aokoro.Settings.Editor
{
    [CustomPropertyDrawer(typeof(SettingsUILibrary.SettingsUIPrefab))]
    public class SettingsUIPrefabDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => EditorGUIUtility.singleLineHeight;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty typeProperty = property.FindPropertyRelative("type");
            SerializedProperty prefabProperty = property.FindPropertyRelative("prefab");

            Rect typeRect = new Rect(position.x, position.y, position.width * .3f, position.height);
            Rect prefabRect = new Rect(position.x + position.width * .3f + 10, position.y, position.width * .7f - 10, position.height);

            EditorGUI.PropertyField(typeRect, typeProperty, GUIContent.none);
            EditorGUI.PropertyField(prefabRect, prefabProperty, GUIContent.none);

            property.serializedObject.ApplyModifiedProperties();
        }
    }
}
