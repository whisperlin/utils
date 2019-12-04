using UnityEditor;
using UnityEngine;
using CinemaDirector;

[CustomEditor(typeof(CinemaAudio))]
public class CinemaAudioInspector : Editor
{
    // Properties
    private SerializedObject cinemaAudio;

    private SerializedProperty firetime;
    private SerializedProperty duration;
    private SerializedProperty inTime;
    private SerializedProperty outTime;
    private SerializedProperty itemLength;

    private const string ERROR_MSG = "CinemaAudio requires an AudioSource component with an assigned Audio Clip.";

    /// <summary>
    /// On inspector enable, load serialized objects
    /// </summary>
    public void OnEnable()
    {
        cinemaAudio = new SerializedObject(this.target);

        this.firetime = cinemaAudio.FindProperty("firetime");
        this.duration = cinemaAudio.FindProperty("duration");
        this.inTime = cinemaAudio.FindProperty("inTime");
        this.outTime = cinemaAudio.FindProperty("outTime");
        this.itemLength = cinemaAudio.FindProperty("itemLength");
    }

    /// <summary>
    /// Update and Draw the inspector
    /// </summary>
    public override void OnInspectorGUI()
    {
        cinemaAudio.Update();

        CinemaAudio audio = (target as CinemaAudio);
        AudioSource audioSource = audio.gameObject.GetComponent<AudioSource>();

        EditorGUILayout.PropertyField(firetime);
        EditorGUILayout.PropertyField(duration);
        EditorGUILayout.PropertyField(inTime);
        EditorGUILayout.PropertyField(outTime);
        
        if (audioSource == null || audioSource.clip == null)
        {
            EditorGUILayout.HelpBox(ERROR_MSG, MessageType.Error);
        }

        AudioClip audioClip = audioSource.clip;

        if (audioClip.length != itemLength.floatValue)
        {
            itemLength.floatValue = audioClip.length;
            outTime.floatValue = Mathf.Min(outTime.floatValue, itemLength.floatValue);
            duration.floatValue = outTime.floatValue - inTime.floatValue;
        }

        cinemaAudio.ApplyModifiedProperties();
    }
}
