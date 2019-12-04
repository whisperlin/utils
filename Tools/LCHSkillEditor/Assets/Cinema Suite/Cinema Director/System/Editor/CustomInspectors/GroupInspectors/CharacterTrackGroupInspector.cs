using UnityEditor;
using UnityEngine;
using CinemaDirector;
using CinemaDirectorControl.Utility;

/// <summary>
/// A custom inspector for a cutscene.
/// </summary>
[CustomEditor(typeof(CharacterTrackGroup), true)]
public class CharacterTrackGroupInspector : Editor
{
    private SerializedProperty actor;
    private SerializedProperty optimizable;
    private SerializedProperty editorRevert;
    private SerializedProperty runtimeRevert;

    private bool containerFoldout = true;
    private Texture inspectorIcon = null;

    #region Language
    //GUIContent ordinalContent = new GUIContent("Ordinal", "The ordinal value of this container, for sorting containers in the timeline.");
    GUIContent addTrackContent = new GUIContent("Add New Track", "Add a new track to this actor track group.");

    GUIContent tracksContent = new GUIContent("Actor Tracks", "The tracks associated with this Actor Group.");
    #endregion

    /// <summary>
    /// Load texture assets on awake.
    /// </summary>
    private void Awake()
    {
        if (inspectorIcon == null)
        {
            inspectorIcon = EditorGUIUtility.Load("Cinema Suite/Cinema Director/Director_InspectorIcon.png") as Texture;
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
        this.actor = base.serializedObject.FindProperty("actor");
        this.optimizable = serializedObject.FindProperty("canOptimize");
        this.editorRevert = serializedObject.FindProperty("editorRevertMode");
        this.runtimeRevert = serializedObject.FindProperty("runtimeRevertMode");
    }

    /// <summary>
    /// Draw the inspector
    /// </summary>
    public override void OnInspectorGUI()
    {
        base.serializedObject.Update();

        ActorTrackGroup actorGroup = base.serializedObject.targetObject as ActorTrackGroup;
        TimelineTrack[] tracks = actorGroup.GetTracks();

        Cutscene cutscene = actorGroup.Cutscene;

        bool isCutsceneActive = false;
        if (cutscene == null)
        {
            EditorGUILayout.HelpBox("Track Group must be a child of a Cutscene in the hierarchy", MessageType.Error);
        }
        else
        {
            isCutsceneActive = !(cutscene.State == Cutscene.CutsceneState.Inactive);
            if (isCutsceneActive)
            {
                EditorGUILayout.HelpBox("Cutscene is Active. Actors cannot be altered at the moment.", MessageType.Info);
            }
        }

        GUI.enabled = !isCutsceneActive;
        EditorGUILayout.PropertyField(actor);
        GUI.enabled = true;

        EditorGUILayout.PropertyField(editorRevert);
        EditorGUILayout.PropertyField(runtimeRevert);
        EditorGUILayout.PropertyField(optimizable);
        if (tracks.Length > 0)
        {
            containerFoldout = EditorGUILayout.Foldout(containerFoldout, tracksContent);
            if (containerFoldout)
            {
                EditorGUI.indentLevel++;

                for (int i = 0; i < tracks.Length; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    tracks[i].name = EditorGUILayout.TextField(tracks[i].name);
                    if (GUILayout.Button(inspectorIcon, GUILayout.Width(24)))
                    {
                        Selection.activeObject = tracks[i];
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUI.indentLevel--;
            }
        }
        if (GUILayout.Button(addTrackContent))
        {
            CutsceneControlHelper.ShowAddTrackContextMenu(actorGroup);
        }

        base.serializedObject.ApplyModifiedProperties();
    }
}
