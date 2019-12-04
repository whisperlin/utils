using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using CinemaDirector;

[CustomEditor(typeof(LoadLevelEvent))]
public class LoadLevelEventInspector : Editor
{
    // Properties
    private SerializedObject loadLevelEvent;
    private SerializedProperty loadLevelType;
    private SerializedProperty loadLevelBy;
    private SerializedProperty loadLevelIndex;
    private SerializedProperty loadLevelName;

    public void OnEnable()
    {
        loadLevelEvent = new SerializedObject(this.target);
        this.loadLevelType = loadLevelEvent.FindProperty("Type");
        this.loadLevelBy = loadLevelEvent.FindProperty("Argument");
        this.loadLevelIndex = loadLevelEvent.FindProperty("Level");
        this.loadLevelName = loadLevelEvent.FindProperty("LevelName");
    }

    public override void OnInspectorGUI()
    {
        loadLevelEvent.Update();

        EditorGUILayout.PropertyField(loadLevelType);
        EditorGUILayout.PropertyField(loadLevelBy);

        if ((LoadLevelEvent.LoadLevelArgument)loadLevelBy.enumValueIndex == LoadLevelEvent.LoadLevelArgument.ByIndex)
        {
            EditorGUILayout.PropertyField(loadLevelIndex);
        }
        else
        {
            EditorGUILayout.PropertyField(loadLevelName);
        }

        loadLevelEvent.ApplyModifiedProperties();
    }
}
