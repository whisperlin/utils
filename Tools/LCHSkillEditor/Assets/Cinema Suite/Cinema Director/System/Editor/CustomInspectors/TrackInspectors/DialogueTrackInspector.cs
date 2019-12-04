// Cinema Suite
using CinemaDirector;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Custom Inspector for Dialogue Tracks.
/// </summary>
[CustomEditor(typeof(DialogueTrack))]
public class DialogueTrackInspector : Editor
{
    // Properties
    private SerializedObject dialogueTrack;
    private SerializedProperty canOptimize;
    private SerializedProperty playbackMode;
    private SerializedProperty anchor;

    /// <summary>
    /// On inspector enable, load serialized objects
    /// </summary>
    public void OnEnable()
    {
        dialogueTrack = new SerializedObject(this.target);
        canOptimize = dialogueTrack.FindProperty("canOptimize");
        anchor = dialogueTrack.FindProperty("anchor");
        playbackMode = dialogueTrack.FindProperty("PlaybackMode");
    }

    /// <summary>
    /// Update and Draw the inspector
    /// </summary>
    public override void OnInspectorGUI()
    {
        dialogueTrack.Update();

        EditorGUILayout.PropertyField(playbackMode, new GUIContent("Playback Mode", "Options for when this Track will execute."));
        EditorGUILayout.PropertyField(anchor, new GUIContent("Anchor","Optional transform to anchor dialogue track to. If blank, will default to Actor."));
        EditorGUILayout.PropertyField(canOptimize, new GUIContent("Optimizable","Enable if track does not have track items dynamically added at runtime."));
       
        dialogueTrack.ApplyModifiedProperties();
    }
}
