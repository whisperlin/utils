using UnityEditor;
using UnityEngine;
using CinemaDirector;

[CustomEditor(typeof(AudioTrack))]
public class AudioTrackInspector : Editor
{
    // Properties
    private SerializedObject audioTrack;

    private GUIContent addAudio = new GUIContent("Add Audio Item", "Add a new audio clip to this track.");
    /// <summary>
    /// On inspector enable, load serialized objects
    /// </summary>
    public void OnEnable()
    {
        audioTrack = new SerializedObject(this.target);
    }

    /// <summary>
    /// Update and Draw the inspector
    /// </summary>
    public override void OnInspectorGUI()
    {
        audioTrack.Update();
        CinemaAudio[] audio = (target as AudioTrack).AudioClips;
        for (int i = 0; i < audio.Length; i++)
        {
            EditorGUILayout.ObjectField(audio[i].name, audio[i], typeof(CinemaAudio), true);
        }

        if (GUILayout.Button(addAudio))
        {
            string label = DirectorHelper.getCutsceneItemName("Audio Item", typeof(CinemaAudio));
            GameObject audioItem = new GameObject(label, new System.Type[]{typeof(CinemaAudio), typeof(AudioSource)});
            audioItem.transform.parent = (this.target as AudioTrack).transform;
        }

        audioTrack.ApplyModifiedProperties();
    }
}
