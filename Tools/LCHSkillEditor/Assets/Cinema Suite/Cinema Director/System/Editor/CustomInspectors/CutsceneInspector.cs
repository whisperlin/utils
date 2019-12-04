using UnityEditor;
using UnityEngine;
using CinemaDirector;
using System;
using CinemaDirectorControl.Utility;

/// <summary>
/// A custom inspector for a cutscene.
/// </summary>
[CustomEditor(typeof(Cutscene))]
public class CutsceneInspector : Editor
{
    private SerializedProperty duration;
    private SerializedProperty isLooping;
    private SerializedProperty isSkippable;
    private SerializedProperty canOptimize;

    private SerializedProperty resetPositionOnEnd;

    //private SerializedProperty runningTime;
    //private SerializedProperty playbackSpeed;
    private bool containerFoldout = true;

    private Texture inspectorIcon = null;

    #region Language
        GUIContent durationContent = new GUIContent("Duration", "The duration of the cutscene in seconds.");
        GUIContent loopingContent = new GUIContent("Loop", "Will the Cutscene loop when finished playing.");
        GUIContent skippableContent = new GUIContent("Skippable", "Can the Cutscene be skipped.");
        GUIContent optimizeContent = new GUIContent("Optimize", "Enable when Cutscene does not have Track Groups added/removed during playtime.");

        GUIContent groupsContent = new GUIContent("Track Groups", "Organizational units of a cutscene.");
        GUIContent addGroupContent = new GUIContent("Add Group", "Add a new container to the cutscene.");

        GUIContent resetPositionOnEndContent = new GUIContent("resetPositionOnEnd", "是否在结束时重置相机位置");
    
    #endregion

    /// <summary>
    /// Load texture assets on awake.
    /// </summary>
    private void Awake()
    {
        if (inspectorIcon == null)
        {
            inspectorIcon = EditorGUIUtility.Load("Cinema Suite/Cinema Director/" + "Director_InspectorIcon" + ".png") as Texture;
        }
        if (inspectorIcon == null)
        {
            Debug.Log("Inspector icon missing from Resources folder.");
        }
    }

    /// <summary>
    /// On inspector enable, load the serialized properties
    /// </summary>
    private void OnEnable()
    {
        this.duration = base.serializedObject.FindProperty("duration");
        this.isLooping = base.serializedObject.FindProperty("isLooping");
        this.isSkippable = base.serializedObject.FindProperty("isSkippable");
        this.canOptimize = base.serializedObject.FindProperty("canOptimize");
        this.resetPositionOnEnd = base.serializedObject.FindProperty("resetPositionOnEnd");
    }

    /// <summary>
    /// Draw the inspector
    /// </summary>
    public override void OnInspectorGUI()
    {
        base.serializedObject.Update();

        EditorGUILayout.BeginHorizontal();
        //EditorGUILayout.PrefixLabel(new GUIContent("Director"));
        if (GUILayout.Button("Open Director"))
        {
            DirectorWindow window = EditorWindow.GetWindow(typeof(DirectorWindow)) as DirectorWindow;
            window.FocusCutscene(base.serializedObject.targetObject as Cutscene);
        }
        EditorGUILayout.EndHorizontal();

        

        EditorGUILayout.PropertyField(this.duration, durationContent);
        EditorGUILayout.PropertyField(this.isLooping, loopingContent);
        EditorGUILayout.PropertyField(this.isSkippable, skippableContent);
        EditorGUILayout.PropertyField(this.canOptimize, optimizeContent);
        EditorGUILayout.PropertyField(this.resetPositionOnEnd, resetPositionOnEndContent);

        containerFoldout = EditorGUILayout.Foldout(containerFoldout, groupsContent);

        if (containerFoldout)
        {
            EditorGUI.indentLevel++;
            Cutscene c = base.serializedObject.targetObject as Cutscene;

            for (int i = 0; i < c.TrackGroups.Length; i++)
            {
                TrackGroup container = c.TrackGroups[i];
                EditorGUILayout.BeginHorizontal();
                
                container.name = EditorGUILayout.TextField(container.name);
                //GUILayout.Button("add", GUILayout.Width(16));
                if (GUILayout.Button(inspectorIcon, GUILayout.Width(24)))
                {
                    Selection.activeObject = container;
                }
                //GUILayout.Button("u", GUILayout.Width(16));
                //GUILayout.Button("d", GUILayout.Width(16));
                EditorGUILayout.EndHorizontal();

                //EditorGUILayout.ObjectField(container.name, container, typeof(TrackGroup), true);
            }
            EditorGUI.indentLevel--;
            if (GUILayout.Button(addGroupContent))
            {
                CutsceneControlHelper.ShowAddTrackGroupContextMenu(c);
            }
        }
       
        base.serializedObject.ApplyModifiedProperties();
    }

}
