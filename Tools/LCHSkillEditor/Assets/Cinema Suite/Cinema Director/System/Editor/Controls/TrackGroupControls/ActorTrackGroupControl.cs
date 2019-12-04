using UnityEditor;
using UnityEngine;
using CinemaDirector;

[CutsceneTrackGroupAttribute(typeof(ActorTrackGroup))]
public class ActorTrackGroupControl : GenericTrackGroupControl
{
    public override void Initialize()
    {
        base.Initialize();
        LabelPrefix = styles.ActorGroupIcon.normal.background;
    }

    protected override void updateHeaderControl4(Rect position)
    {
        Transform actor = (TrackGroup.Behaviour as ActorTrackGroup).Actor;

        Color temp = GUI.color;

        GUI.color = (actor == null) ? Color.red : Color.green;
        int controlID = GUIUtility.GetControlID("ActorTrackGroupControl".GetHashCode(), FocusType.Passive, position);

        GUI.enabled = !(state.IsInPreviewMode && (actor == null));
        if (GUI.Button(position, string.Empty, styles.PickerStyle))
        {
            if (actor == null)
            {
                EditorGUIUtility.ShowObjectPicker<Transform>(actor, true, string.Empty, controlID);
            }
            else
            {
                Selection.activeGameObject = actor.gameObject;
            }
        }
        GUI.enabled = true;

        if (Event.current.commandName == "ObjectSelectorUpdated")
        {
            if (EditorGUIUtility.GetObjectPickerControlID() == controlID)
            {
                GameObject pickedObject = EditorGUIUtility.GetObjectPickerObject() as GameObject;
                if (pickedObject != null)
                {
                    ActorTrackGroup atg = (TrackGroup.Behaviour as ActorTrackGroup);
                    Undo.RecordObject(atg, string.Format("Changed {0}", atg.name));
                    atg.Actor = pickedObject.transform;
                }
            }
        }
        GUI.color = temp;
    }

    private void focusActor()
    {
        Selection.activeTransform = (TrackGroup.Behaviour as ActorTrackGroup).Actor;
    }
}