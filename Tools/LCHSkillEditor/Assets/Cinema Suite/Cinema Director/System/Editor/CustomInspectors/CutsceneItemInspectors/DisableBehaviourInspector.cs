using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using CinemaDirector;

[CustomEditor(typeof(DisableBehaviour))]
public class DisableBehaviourInspector : Editor
{
    // Properties
    private SerializedObject disableBehaviour;
    private SerializedProperty fireTime;
    private SerializedProperty componentsProperty;
    private SerializedProperty editorRevert;
    private SerializedProperty runtimeRevert;
    private int componentSelection;

    #region Language
    GUIContent firetimeContent = new GUIContent("Firetime", "The time in seconds at which this event is fired.");
    #endregion

    public void OnEnable()
    {
        disableBehaviour = new SerializedObject(this.target);
        this.fireTime = disableBehaviour.FindProperty("firetime");
        this.componentsProperty = disableBehaviour.FindProperty("Behaviour");
        this.editorRevert = disableBehaviour.FindProperty("editorRevertMode");
        this.runtimeRevert = disableBehaviour.FindProperty("runtimeRevertMode");
        Component currentComponent = componentsProperty.objectReferenceValue as Component;

        DisableBehaviour behaviour = (target as DisableBehaviour);
        if (behaviour == null || behaviour.ActorTrackGroup == null || behaviour.ActorTrackGroup.Actor == null)
        {
            return;
        }
        GameObject actor = behaviour.ActorTrackGroup.Actor.gameObject;

        Component[] behaviours = DirectorHelper.getEnableableComponents(actor);
        for (int j = 0; j < behaviours.Length; j++)
        {
            if (behaviours[j] == currentComponent)
            {
                componentSelection = j;
            }
        }
    }

    public override void OnInspectorGUI()
    {
        disableBehaviour.Update();

        DisableBehaviour behaviour = (target as DisableBehaviour);
        GameObject actor = behaviour.ActorTrackGroup.Actor.gameObject;

        EditorGUILayout.PropertyField(this.fireTime, firetimeContent);

        List<GUIContent> componentSelectionList = new List<GUIContent>();
        Component[] behaviours = DirectorHelper.getEnableableComponents(actor);
        for (int i = 0; i < behaviours.Length; i++)
        {
            Component b = behaviours[i];
            componentSelectionList.Add(new GUIContent(b.GetType().Name));
            if (componentsProperty.objectReferenceValue as Component == b)
            {
                componentSelection = i;
            }
        }
        componentSelection = EditorGUILayout.Popup(new GUIContent("Behaviour"), componentSelection, componentSelectionList.ToArray());
        componentsProperty.objectReferenceValue = behaviours[componentSelection];

        EditorGUILayout.PropertyField(editorRevert);
        EditorGUILayout.PropertyField(runtimeRevert);

        disableBehaviour.ApplyModifiedProperties();
    }
}
