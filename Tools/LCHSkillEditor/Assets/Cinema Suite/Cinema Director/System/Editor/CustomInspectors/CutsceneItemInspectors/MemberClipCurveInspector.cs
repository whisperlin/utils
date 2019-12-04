using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using CinemaDirector;

[CustomPropertyDrawer(typeof(MemberClipCurveData))]
public class MemberClipCurveDrawer : PropertyDrawer 
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //EditorGUI.BeginProperty(position, GUIContent.none, property);
        //SerializedProperty memberProperty = property.FindPropertyRelative("PropertyName");
        //SerializedProperty typeProperty = property.FindPropertyRelative("Type");
        SerializedProperty propertyProperty = property.FindPropertyRelative("PropertyType");
        PropertyTypeInfo propertyTypeInfo = (PropertyTypeInfo) propertyProperty.enumValueIndex;
        SerializedProperty curve1Property = property.FindPropertyRelative("Curve1");
        SerializedProperty curve2Property = property.FindPropertyRelative("Curve2");
        SerializedProperty curve3Property = property.FindPropertyRelative("Curve3");
        SerializedProperty curve4Property = property.FindPropertyRelative("Curve4");

        int count = UnityPropertyTypeInfo.GetCurveCount(propertyTypeInfo);

        EditorGUI.indentLevel++;
            if(count > 0)
            EditorGUILayout.PropertyField(curve1Property);
            if (count > 1)
            EditorGUILayout.PropertyField(curve2Property);
            if (count > 2)
            EditorGUILayout.PropertyField(curve3Property);
            if (count > 3)
            EditorGUILayout.PropertyField(curve4Property);
        EditorGUI.indentLevel--;

        //EditorGUI.EndProperty();
    }
}
