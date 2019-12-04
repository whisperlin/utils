using System;
using System.Collections.Generic;
using System.Reflection;

using UnityEditor;
using UnityEngine;
using CinemaDirector;

/// <summary>
/// Actor Curve Track Control
/// </summary>
[CutsceneTrackAttribute(typeof(MultiCurveTrack))]
public class MultiActorCurveTrackControl : CinemaCurveTrackControl
{
    private TimelineTrack track;

    public override void Initialize()
    {
        base.Initialize();
        isExpanded = true;
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

    protected override void updateHeaderControl3(UnityEngine.Rect position)
    {
        MultiCurveTrack track = TargetTrack.Behaviour as MultiCurveTrack;
        if (track == null) return;

        Color temp = GUI.color;
        GUI.color = (track.TimelineItems.Length > 0) ? Color.green : Color.red;

        if (GUI.Button(position, string.Empty, TrackGroupControl.styles.AddIcon))
        {
            addNewCurveItem(track);
        }
        GUI.color = temp;
    }

    private void addNewCurveItem(MultiCurveTrack track)
    {
        Undo.RegisterCreatedObjectUndo(CutsceneItemFactory.CreateMultiActorClipCurve(track), "Created Multi Actor Clip Curve");
    }

    protected override void showBodyContextMenu(Event evt)
    {
        MultiCurveTrack itemTrack = TargetTrack.Behaviour as MultiCurveTrack;
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
        public MultiCurveTrack track;

        public PasteContext(Vector2 mousePosition, MultiCurveTrack track)
        {
            this.mousePosition = mousePosition;
            this.track = track;
        }
    }
}
