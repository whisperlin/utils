using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using CinemaDirector;

[CustomEditor(typeof(CurveTrack))]
public class CurveTrackInspector : Editor
{
    // Properties
    private SerializedObject curveTrack;

    private GUIContent addClip = new GUIContent("Add Clip Curve", "Add a new clip curve to this track.");

    /// <summary>
    /// On inspector enable, load serialized objects
    /// </summary>
    public void OnEnable()
    {
        curveTrack = new SerializedObject(this.target);
    }

    /// <summary>
    /// Update and Draw the inspector
    /// </summary>
    public override void OnInspectorGUI()
    {
        curveTrack.Update();
        TimelineItem[] items = (target as CurveTrack).TimelineItems;
        for (int i = 0; i < items.Length; i++)
        {
            CinemaActorClipCurve clip = items[i] as CinemaActorClipCurve;
            EditorGUILayout.ObjectField(clip.name, clip, typeof(CinemaActorClipCurve), true);
        }

        if (GUILayout.Button(addClip))
        {
            Undo.RegisterCreatedObjectUndo(CutsceneItemFactory.CreateActorClipCurve((target as CurveTrack)).gameObject, "Create Curve Clip");
        }

        curveTrack.ApplyModifiedProperties();
    }
}
