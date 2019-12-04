// Cinema Suite Inc. 2014
using UnityEditor;
using UnityEngine;
using CinemaDirector;

[CustomEditor(typeof(ShotTrack))]
public class ShotTrackInspector : Editor
{
    // Properties
    private SerializedObject visualTrack;
    private bool shotFoldout = true;

    /// <summary>
    /// On inspector enable, load serialized objects
    /// </summary>
    public void OnEnable()
    {
        visualTrack = new SerializedObject(this.target);
    }

    /// <summary>
    /// Update and Draw the inspector
    /// </summary>
    public override void OnInspectorGUI()
    {
        visualTrack.Update();

        EditorGUILayout.Foldout(shotFoldout, "Shot List");
        ShotTrack track = base.serializedObject.targetObject as ShotTrack;
        TimelineItem[] items = track.TimelineItems;
        for (int i = 0; i < items.Length; i++)
        {
            CinemaGlobalAction shot = items[i] as CinemaGlobalAction;
            shot.name = EditorGUILayout.TextField(new GUIContent("Shot Name"), shot.name);

            EditorGUI.indentLevel++;


            // Check if it is an actor event.
            CinemaShot cinemaShot = shot as CinemaShot;
            if (cinemaShot != null)
            {
                cinemaShot.shotCamera = EditorGUILayout.ObjectField(new GUIContent("Camera"), cinemaShot.shotCamera, typeof(Camera), true) as Camera;
            }
            else
            {
                // Display something for non-default shots
            }

            shot.Firetime = EditorGUILayout.FloatField(new GUIContent("Cut Time"), shot.Firetime);
            shot.Duration = EditorGUILayout.FloatField(new GUIContent("Shot Length"), shot.Duration);
            EditorGUI.indentLevel--;



        }

        if(GUILayout.Button("Add New Shot"))
        {
            CutsceneItemFactory.CreateNewShot(track);
        }
        visualTrack.ApplyModifiedProperties();

    }
}
