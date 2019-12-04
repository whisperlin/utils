using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using CinemaDirector;

[CustomEditor(typeof(EnableBehaviour))]
public class EnableBehaviourInspector : Editor
{
    // Properties
    private SerializedObject enableBehaviour;
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
        enableBehaviour = new SerializedObject(this.target);
        this.fireTime = enableBehaviour.FindProperty("firetime");
        this.componentsProperty = enableBehaviour.FindProperty("Behaviour");
        this.editorRevert = enableBehaviour.FindProperty("editorRevertMode");
        this.runtimeRevert = enableBehaviour.FindProperty("runtimeRevertMode");
        Component currentComponent = componentsProperty.objectReferenceValue as Component;

        EnableBehaviour behaviour = (target as EnableBehaviour);
        if (behaviour == null || behaviour.ActorTrackGroup == null || behaviour.ActorTrackGroup.Actor == null)
        {
            return;
        }
        GameObject actor = behaviour.ActorTrackGroup.Actor.gameObject;

        Component[] components = DirectorHelper.getEnableableComponents(actor);
        for (int j = 0; j < components.Length; j++)
        {
            if (components[j] == currentComponent)
            {
                componentSelection = j;
            }
        }
    }

    public override void OnInspectorGUI()
    {
        enableBehaviour.Update();

        EnableBehaviour behaviour = (target as EnableBehaviour);
        GameObject actor = behaviour.ActorTrackGroup.Actor.gameObject;

        EditorGUILayout.PropertyField(this.fireTime, firetimeContent);

        List<GUIContent> componentSelectionList = new List<GUIContent>();
        Component[] components = DirectorHelper.getEnableableComponents(actor);
        for (int i = 0; i < components.Length; i++)
        {
            Component component = components[i];
            componentSelectionList.Add(new GUIContent(component.GetType().Name));
            if (componentsProperty.objectReferenceValue as Component == component)
            {
                componentSelection = i;
            }
        }
        componentSelection = EditorGUILayout.Popup(new GUIContent("Behaviour"), componentSelection, componentSelectionList.ToArray());
        componentsProperty.objectReferenceValue = components[componentSelection];

        EditorGUILayout.PropertyField(editorRevert);
        EditorGUILayout.PropertyField(runtimeRevert);

        enableBehaviour.ApplyModifiedProperties();
    }
}
