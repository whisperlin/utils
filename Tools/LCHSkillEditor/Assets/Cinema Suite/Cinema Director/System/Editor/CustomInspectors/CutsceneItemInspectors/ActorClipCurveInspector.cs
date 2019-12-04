using CinemaDirector;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CinemaActorClipCurve))]
public class ActorClipCurveInspector : Editor
{
    // Properties
    private SerializedObject actorClipCurve;
    private SerializedProperty editorRevert;
    private SerializedProperty runtimeRevert;

    private int componentSeletion = 0;
    private int propertySelection = 0;
    private bool isDataFolded = true;
    private bool isNewCurveFolded = true;
    private GUIContent dataContent = new GUIContent("Curves");
    private GUIContent addContent = new GUIContent("Add new curves");

    [Multiline]
    private const string ERROR_MSG = "CinemaActorClipCurve requires an \nActorTrack as a parent object \nwith an assigned actor.";
    public void OnEnable()
    {
        actorClipCurve = new SerializedObject(this.target);
        this.editorRevert = actorClipCurve.FindProperty("editorRevertMode");
        this.runtimeRevert = actorClipCurve.FindProperty("runtimeRevertMode");
    }

    public override void OnInspectorGUI()
    {
        actorClipCurve.Update();
        CinemaActorClipCurve clipCurveGameObject = (target as CinemaActorClipCurve);
        
        if (clipCurveGameObject == null || clipCurveGameObject.Actor == null)
        {
            EditorGUILayout.HelpBox(ERROR_MSG, UnityEditor.MessageType.Error);
            return;
        }

        GameObject actor = clipCurveGameObject.Actor.gameObject;

        List<KeyValuePair<string, string>> currentCurves = new List<KeyValuePair<string, string>>();

        EditorGUILayout.PropertyField(editorRevert);
        EditorGUILayout.PropertyField(runtimeRevert);

        SerializedProperty curveData = actorClipCurve.FindProperty("curveData");
        if (curveData.arraySize > 0)
        {
            isDataFolded = EditorGUILayout.Foldout(isDataFolded, dataContent);
            if (isDataFolded)
            {
                for (int i = 0; i < curveData.arraySize; i++)
                {
                    SerializedProperty member = curveData.GetArrayElementAtIndex(i);
                    SerializedProperty typeProperty = member.FindPropertyRelative("Type");
                    SerializedProperty memberProperty = member.FindPropertyRelative("PropertyName");

                    KeyValuePair<string, string> curveStrings = new KeyValuePair<string, string>(typeProperty.stringValue, memberProperty.stringValue);
                    currentCurves.Add(curveStrings);

                    Component c = actor.GetComponent(typeProperty.stringValue);

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(new GUIContent(string.Format("{0}.{1}", typeProperty.stringValue, DirectorHelper.GetUserFriendlyName(typeProperty.stringValue, memberProperty.stringValue)), EditorGUIUtility.ObjectContent(c, c.GetType()).image));
                    if (GUILayout.Button("-", GUILayout.Width(24f)))
                    {
                        curveData.DeleteArrayElementAtIndex(i);
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            GUILayout.Space(5);
        }

        isNewCurveFolded = EditorGUILayout.Foldout(isNewCurveFolded, addContent);

        if (isNewCurveFolded)
        {
            List<GUIContent> componentSelectionList = new List<GUIContent>();
            List<GUIContent> propertySelectionList = new List<GUIContent>();

            Component[] components = DirectorHelper.getValidComponents(actor);
            for (int i = 0; i < components.Length; i++)
            {
                Component component = components[i];

                if (component != null)
                    componentSelectionList.Add(new GUIContent(component.GetType().Name));
            }

            componentSeletion = EditorGUILayout.Popup(new GUIContent("Component"), componentSeletion, componentSelectionList.ToArray());
            MemberInfo[] members = DirectorHelper.getValidMembers(components[componentSeletion]);
            List<MemberInfo> newMembers = new List<MemberInfo>();
            for (int i = 0; i < members.Length; i++)
            {
                MemberInfo memberInfo = members[i];
                if (!currentCurves.Contains(new KeyValuePair<string, string>(components[componentSeletion].GetType().Name, memberInfo.Name)))
                {
                    newMembers.Add(memberInfo);
                }
            }
            members = newMembers.ToArray();

            for (int i = 0; i < members.Length; i++)
            {
                MemberInfo memberInfo = members[i];
                string name = DirectorHelper.GetUserFriendlyName(components[componentSeletion], memberInfo);
                propertySelectionList.Add(new GUIContent(name));
            }
            propertySelection = EditorGUILayout.Popup(new GUIContent("Property"), propertySelection, propertySelectionList.ToArray());

            if (GUILayout.Button("Add Curve") && members.Length > 0)
            {
                Type t = null;
                PropertyInfo property = members[propertySelection] as PropertyInfo;
                FieldInfo field = members[propertySelection] as FieldInfo;
                bool isProperty = false;
                if (property != null)
                {
                    t = property.PropertyType;
                    isProperty = true;
                }
                else if (field != null)
                {
                    t = field.FieldType;
                    isProperty = false;
                }
                clipCurveGameObject.AddClipCurveData(components[componentSeletion], members[propertySelection].Name, isProperty, t);
            }
        }



        actorClipCurve.ApplyModifiedProperties();
    }

    public void OnSceneGUI()
    {
        CinemaActorClipCurve clipCurveGameObject = (target as CinemaActorClipCurve);
        for (int i = 0; i < clipCurveGameObject.CurveData.Count; i++)
        {
            MemberClipCurveData data = clipCurveGameObject.CurveData[i];
            if (data.Type == "Transform" && data.PropertyName == "localPosition")
            {
                //Vector3 position = new Vector3(data.Curve1[0].value, data.Curve2[0].value, data.Curve3[0].value);

                //Handles.DrawSphere(0, position, Quaternion.identity, 10);
            }
        }
    }
}
