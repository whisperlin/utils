using UnityEditor;
using UnityEngine;
using CinemaDirector;
using CinemaDirectorControl.Utility;

/// <summary>
/// A custom inspector for a cutscene.
/// </summary>
[CustomEditor(typeof(MultiActorTrackGroup), true)]
public class MultiActorTrackGroupInspector : Editor
{
    //private SerializedProperty actors;

    #region Language;

    GUIContent addTrackContent = new GUIContent("Add New Track", "Add a new track to this multi-actor track group.");

    #endregion

    /// <summary>
    /// On inspector enable, load the serialized properties
    /// </summary>
    private void OnEnable()
    {
        //this.actors = base.serializedObject.FindProperty("actors");
    }

    /// <summary>
    /// Draw the inspector
    /// </summary>
    public override void OnInspectorGUI()
    {
        base.DrawDefaultInspector();

        MultiActorTrackGroup multiActorGroup = base.serializedObject.targetObject as MultiActorTrackGroup;

        //base.serializedObject.Update();

        //EditorGUILayout.PropertyField(actors, actorsContent);

        if (GUILayout.Button(addTrackContent))
        {
            CutsceneControlHelper.ShowAddTrackContextMenu(multiActorGroup);
        }

        //base.serializedObject.ApplyModifiedProperties();
    }
}
