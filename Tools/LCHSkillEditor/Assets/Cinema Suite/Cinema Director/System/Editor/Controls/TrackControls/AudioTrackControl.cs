// Cinema Suite
using UnityEditor;
using UnityEngine;
using CinemaDirector;
using System.Reflection;

/// <summary>
/// Audio Track Control
/// </summary>
[CutsceneTrackAttribute(typeof(AudioTrack))]
public class AudioTrackControl : GenericTrackControl
{
    public override void Initialize()
    {
        base.Initialize();
        isExpanded = true;
        expandedSize = 3;
    }

    public override void UpdateTrackContents(DirectorControlState state, Rect position)
    {
        handleDragInteraction(position, TargetTrack.Behaviour as AudioTrack, state.Translation, state.Scale);
        base.UpdateTrackContents(state, position);
    }

    private void handleDragInteraction(Rect position, AudioTrack track, Vector2 translation, Vector2 scale)
    {
        Rect controlBackground = new Rect(0, 0, position.width, position.height);
        switch (Event.current.type)
        {
            case EventType.DragUpdated:
                if (controlBackground.Contains(Event.current.mousePosition))
                {
                    bool audioFound = false;
                    Object[] objRefs = DragAndDrop.objectReferences;
                    for (int i = 0; i < objRefs.Length; i++)
                    {
                        AudioClip clip = objRefs[i] as AudioClip;
                        if (clip != null)
                        {
                            audioFound = true;
                            break;
                        }
                    }
                    if (audioFound)
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Link;
                        Event.current.Use();
                    }
                }
                break;
            case EventType.DragPerform:
                if (controlBackground.Contains(Event.current.mousePosition))
                {
                    AudioClip clip = null;
                    Object[] objRefs = DragAndDrop.objectReferences;
                    for (int i = 0; i < objRefs.Length; i++)
                    {
                        AudioClip audioClip = objRefs[i] as AudioClip;
                        if (audioClip != null)
                        {
                            clip = audioClip;
                            break;
                        }
                    }
                    if (clip != null)
                    {
                        float fireTime = (Event.current.mousePosition.x - translation.x) / scale.x;
                        CinemaAudio ca = CutsceneItemFactory.CreateCinemaAudio(track, clip, fireTime);
                        Undo.RegisterCreatedObjectUndo(ca, string.Format("Created {0}", ca.name));
                        Event.current.Use();
                    }
                }
                break;
        }
    }

}
