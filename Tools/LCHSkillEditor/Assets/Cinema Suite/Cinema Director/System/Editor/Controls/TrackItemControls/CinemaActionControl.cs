using UnityEngine;
using System.Collections;
using UnityEditor;
using CinemaDirector;

[CutsceneItemControlAttribute(typeof(TimelineAction))]
public class CinemaActionControl : ActionItemControl
{
    public CinemaActionControl()
    {
        base.AlterAction += CinemaActionControl_AlterAction;
    }

    void CinemaActionControl_AlterAction(object sender, ActionItemEventArgs e)
    {
        TimelineAction action = e.actionItem as TimelineAction;
        if (action == null) return;

        if (e.duration <= 0)
        {
            deleteItem(e.actionItem);
        }
        else
        {
            Undo.RecordObject(e.actionItem, string.Format("Change {0}", action.name));
            action.Firetime = e.firetime;
            action.Duration = e.duration;
        }
    }
}
