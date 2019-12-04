using System;
using System.Collections.Generic;
using System.Reflection;

using UnityEditor;
using UnityEngine;
using CinemaDirector;

/// <summary>
/// Actor Curve Track Control
/// </summary>
[CutsceneTrackAttribute(typeof(CurveTrack))]
public class ActorCurveTrackControl : CinemaCurveTrackControl
{
    private TimelineTrack track;

    public override void Initialize()
    {
        base.Initialize();
        isExpanded = true;
    }

    protected override void updateHeaderControl3(UnityEngine.Rect position)
    {
        CurveTrack track = TargetTrack.Behaviour as CurveTrack;
        if (track == null) return;

        Color temp = GUI.color;
        GUI.color = (track.TimelineItems.Length > 0) ? Color.green : Color.red;

        if (GUI.Button(position, string.Empty, TrackGroupControl.styles.AddIcon))
        {
            addNewCurveItem(track);
        }
        GUI.color = temp;
    }

    protected override void updateHeaderControl4(UnityEngine.Rect position)
    {
        track = this.Behaviour.gameObject.GetComponent<TimelineTrack>();

        if (!track.lockedStatus)
        {
            if (GUI.Button(position, string.Empty, TrackGroupControl.styles.UnlockIconSM))
                track.lockedStatus = true;
        }
        else
        {
            if (GUI.Button(position, string.Empty, TrackGroupControl.styles.LockIconSM))
                track.lockedStatus = false;
        }
    }



    private void addNewCurveItem(CurveTrack track)
    {
        Undo.RegisterCreatedObjectUndo(CutsceneItemFactory.CreateActorClipCurve(track), "Created Actor Clip Curve");
    }

    protected override void showBodyContextMenu(Event evt)
    {
        CurveTrack itemTrack = TargetTrack.Behaviour as CurveTrack;
        if (itemTrack == null) return;

        Behaviour b = DirectorCopyPaste.Peek();

        PasteContext pasteContext = new PasteContext(evt.mousePosition, itemTrack);
        GenericMenu createMenu = new GenericMenu();
        if (b != null && DirectorHelper.IsTrackItemValidForTrack(b, itemTrack))
        {
            createMenu.AddItem(new GUIContent("Paste"), false, pasteItem, pasteContext);
        }
        else
        {
            createMenu.AddDisabledItem(new GUIContent("Paste"));
        }
        createMenu.ShowAsContext();
    }

    private void pasteItem(object userData)
    {
        PasteContext data = userData as PasteContext;
        if (data != null)
        {
            float firetime = (data.mousePosition.x - state.Translation.x) / state.Scale.x;
            GameObject clone = DirectorCopyPaste.Paste(data.track.transform);

            CinemaClipCurve clipCurve = clone.GetComponent<CinemaClipCurve>();
            clipCurve.TranslateCurves(firetime - clipCurve.Firetime);

            Undo.RegisterCreatedObjectUndo(clone, "Pasted " + clone.name);
        }
    }

    private class PasteContext
    {
        public Vector2 mousePosition;
        public CurveTrack track;

        public PasteContext(Vector2 mousePosition, CurveTrack track)
        {
            this.mousePosition = mousePosition;
            this.track = track;
        }
    }
}
