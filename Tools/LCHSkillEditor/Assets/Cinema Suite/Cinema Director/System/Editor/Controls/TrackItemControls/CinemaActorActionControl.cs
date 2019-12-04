using UnityEngine;
using System.Collections;
using UnityEditor;
using CinemaDirector;

[CutsceneItemControlAttribute(typeof(CinemaActorAction))]
public class CinemaActorActionControl : CinemaActionControl
{
    public CinemaActorActionControl()
        : base()
    {
    }

    public override void Draw(DirectorControlState state)
    {
        CinemaActorAction action = Wrapper.Behaviour as CinemaActorAction;
        if (action == null) return;

        if (IsSelected)
        {
            GUI.Box(controlPosition, GUIContent.none, TimelineTrackControl.styles.ActorTrackItemSelectedStyle);
        }
        else
        {
            GUI.Box(controlPosition, GUIContent.none, TimelineTrackControl.styles.ActorTrackItemStyle);
        }
        
        DrawRenameLabel(action.name, controlPosition);
    }
}
