using UnityEngine;
using System.Collections;
using UnityEditor;
using CinemaDirector;

[CutsceneItemControlAttribute(typeof(CinemaGlobalAction))]
public class CinemaGlobalActionControl : CinemaActionControl
{
    public CinemaGlobalActionControl() : base()
    {
    }

    public override void Draw(DirectorControlState state)
    {
        CinemaGlobalAction action = Wrapper.Behaviour as CinemaGlobalAction;
        if (action == null) return;

        if (IsSelected)
        {
            GUI.Box(controlPosition, GUIContent.none, TimelineTrackControl.styles.GlobalTrackItemSelectedStyle);
        }
        else
        {
            GUI.Box(controlPosition, GUIContent.none, TimelineTrackControl.styles.GlobalTrackItemStyle);
        }
        
        DrawRenameLabel(action.name, controlPosition);
    }
}
