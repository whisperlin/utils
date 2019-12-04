using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using CinemaDirector;

[CustomEditor(typeof(CinemaMultiActorCurveClip))]
public class MultiActorClipCurveInspector : Editor
{
    // Properties
    private SerializedObject clipCurve;

    private SerializedProperty propertyType;
    private SerializedProperty componentsProperty;
    private SerializedProperty propertiesProperty;
    private SerializedProperty editorRevert;
    private SerializedProperty runtimeRevert;

    private int[] componentSelection;
    private int[] propertySelection;

    private int actorCount;

    /// <summary>
    /// On inspector enable, load serialized objects
    /// </summary>
    public void OnEnable()
    {
        clipCurve = new SerializedObject(this.target);
        CinemaMultiActorCurveClip curveclipGO = target as CinemaMultiActorCurveClip;

        actorCount = curveclipGO.Actors.Count;
        componentSelection = new int[actorCount];
        propertySelection = new int[actorCount];

        SerializedProperty curveData = clipCurve.FindProperty("curveData");
        if (curveData.arraySize > 0)
        {
            SerializedProperty member = curveData.GetArrayElementAtIndex(0);
            propertyType = member.FindPropertyRelative("PropertyType");
        }

        componentsProperty = clipCurve.FindProperty("Components");
        propertiesProperty = clipCurve.FindProperty("Properties");
        this.editorRevert = clipCurve.FindProperty("editorRevertMode");
        this.runtimeRevert = clipCurve.FindProperty("runtimeRevertMode");

        componentsProperty.arraySize = actorCount;
        propertiesProperty.arraySize = actorCount;

        for (int i = 0; i < actorCount; i++)
        {
            Transform actor = curveclipGO.Actors[i];

            Component currentComponent = componentsProperty.GetArrayElementAtIndex(i).objectReferenceValue as Component;
            string currentProperty = propertiesProperty.GetArrayElementAtIndex(i).stringValue;

            Component[] components = getValidComponents(actor.gameObject);
            for (int j = 0; j < components.Length; j++ )
            {
                if (components[j] == currentComponent)
                {
                    componentSelection[i] = j;
                }
            }

            PropertyInfo[] properties = getValidProperties(components[componentSelection[i]], (PropertyTypeInfo)propertyType.enumValueIndex);
            for (int j = 0; j < properties.Length; j++)
            {
                if (properties[j].Name == currentProperty)
                {
                    propertySelection[i] = j;
                }
            }
        }
    }

    /// <summary>
    /// Update and Draw the inspector
    /// </summary>
    public override void OnInspectorGUI()
    {
        clipCurve.Update();

        EditorGUILayout.PropertyField(editorRevert);
        EditorGUILayout.PropertyField(runtimeRevert);

        SerializedProperty curveData = clipCurve.FindProperty("curveData");
        if (curveData.arraySize > 0)
        {
            CinemaMultiActorCurveClip curveclip = target as CinemaMultiActorCurveClip;

            SerializedProperty member = curveData.GetArrayElementAtIndex(0);
            propertyType = member.FindPropertyRelative("PropertyType");

            PropertyTypeInfo current = (PropertyTypeInfo) propertyType.enumValueIndex;
            PropertyTypeInfo newProperty = (PropertyTypeInfo) EditorGUILayout.EnumPopup(new GUIContent("Property Type"), current);

            if (current != newProperty)
            {
                propertyType.enumValueIndex = (int)newProperty;

                SerializedProperty curve1 = member.FindPropertyRelative("Curve1");
                SerializedProperty curve2 = member.FindPropertyRelative("Curve2");
                SerializedProperty curve3 = member.FindPropertyRelative("Curve3");
                SerializedProperty curve4 = member.FindPropertyRelative("Curve4");

                //SerializedProperty firetime = clipCurve.FindProperty("Firetime");
                //SerializedProperty duration = clipCurve.FindProperty("Duration");

                curve1.animationCurveValue = AnimationCurve.Linear(curveclip.Firetime, 0, curveclip.Duration, 0);
                curve2.animationCurveValue = AnimationCurve.Linear(curveclip.Firetime, 0, curveclip.Duration, 0);
                curve3.animationCurveValue = AnimationCurve.Linear(curveclip.Firetime, 0, curveclip.Duration, 0);
                curve4.animationCurveValue = AnimationCurve.Linear(curveclip.Firetime, 0, curveclip.Duration, 0);
            }

            componentsProperty.arraySize = actorCount;
            propertiesProperty.arraySize = actorCount;

            for (int i = 0; i < actorCount; i++)
            {
                Transform actor = curveclip.Actors[i];
                EditorGUILayout.LabelField(actor.name);
                EditorGUI.indentLevel++;
                List<GUIContent> componentSelectionList = new List<GUIContent>();
                List<GUIContent> propertySelectionList = new List<GUIContent>();

                // Display component selection
                Component[] components = getValidComponents(actor.gameObject);
                for (int j = 0; j < components.Length; j++)
                {
                    componentSelectionList.Add(new GUIContent(components[j].GetType().Name));
                }
                componentSelection[i] = EditorGUILayout.Popup(new GUIContent("Component"), componentSelection[i], componentSelectionList.ToArray());

                componentsProperty.GetArrayElementAtIndex(i).objectReferenceValue = components[componentSelection[i]];

                // Display property selection
                PropertyInfo[] properties = getValidProperties(components[componentSelection[i]], (PropertyTypeInfo)propertyType.enumValueIndex);
                for (int j = 0; j < properties.Length; j++)
                {
                    propertySelectionList.Add(new GUIContent(properties[j].Name));
                }
                Color temp = GUI.color;
                if (propertySelectionList.Count < 1)
                {
                    propertySelectionList.Add(new GUIContent("None"));
                    GUI.color = Color.red;
                }
                propertySelection[i] = EditorGUILayout.Popup(new GUIContent("Property"), propertySelection[i], propertySelectionList.ToArray());

                if (propertySelection[i] > propertySelectionList.Count - 1)
                {
                    propertySelection[i] = 0;
                }
                string selectedProperty = propertySelectionList[propertySelection[i]].text;
                propertiesProperty.GetArrayElementAtIndex(i).stringValue = selectedProperty;
                GUI.color = temp;

                EditorGUI.indentLevel--;
            }
        }

        clipCurve.ApplyModifiedProperties();
    }

    private Component[] getValidComponents(GameObject actor)
    {
        return actor.GetComponents<Component>();
    }

    private PropertyInfo[] getValidProperties(Component component, PropertyTypeInfo propertyTypeInfo)
    {
        List<PropertyInfo> properties = new List<PropertyInfo>();
        PropertyInfo[] pi = component.GetType().GetProperties();
        for (int i = 0; i < pi.Length; i++)
        {
            if (UnityPropertyTypeInfo.GetMappedType(pi[i].PropertyType) == propertyTypeInfo && pi[i].CanWrite)
            {
                properties.Add(pi[i]);
            }
        }
        return properties.ToArray();
    }

}
