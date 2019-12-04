using CinemaDirector;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

/// <summary>
/// A Track Control for handling most Timeline Tracks.
/// </summary>
[CutsceneTrackAttribute(typeof(TimelineTrack))]
public class GenericTrackControl : TimelineTrackControl
{
    private ContextData savedData = null; // Saved data from the object picker.
    private int controlID; // The control ID for this track control.
    private TimelineTrack track;

    /// <summary>
    /// Header Control 4 is typically the "Add" control.
    /// </summary>
    /// <param name="position">The position that this control is drawn at.</param>
    protected override void updateHeaderControl4(UnityEngine.Rect position)
    {
        TimelineTrack track = TargetTrack.Behaviour as TimelineTrack;
        if (track == null) return;

        Color temp = GUI.color;
        GUI.color = (track.GetTimelineItems().Length > 0) ? Color.green : Color.red;

        controlID = GUIUtility.GetControlID(track.GetInstanceID(), FocusType.Passive);

        if (GUI.Button(position, string.Empty, TrackGroupControl.styles.AddIcon))
        {
            // Get the possible items that this track can contain.
            List<Type> trackTypes = track.GetAllowedCutsceneItems();

            if (trackTypes.Count == 1)
            {
                // Only one option, so just create it.
                ContextData data = getContextData(trackTypes[0]);
                if (data.PairedType == null)
                {
                    addCutsceneItem(data);
                }
                else
                {
                    showObjectPicker(data);
                }
            }
            else if (trackTypes.Count > 1)
            {
                // Present context menu for selection.
                GenericMenu createMenu = new GenericMenu();
                for (int i = 0; i < trackTypes.Count; i++)
                {
                    ContextData data = getContextData(trackTypes[i]);

                    createMenu.AddItem(new GUIContent(string.Format("{0}/{1}", data.Category, data.Label)), false, addCutsceneItem, data);
                }
                createMenu.ShowAsContext();
            }
        }

        // Handle the case where the object picker has a value selected.
        if (Event.current.type == EventType.ExecuteCommand && Event.current.commandName == "ObjectSelectorClosed")
        {
            if (EditorGUIUtility.GetObjectPickerControlID() == controlID)
            {
                UnityEngine.Object pickedObject = EditorGUIUtility.GetObjectPickerObject();

                if(pickedObject != null)
                    addCutsceneItem(savedData, pickedObject);

                Event.current.Use();
            }
        }

        GUI.color = temp;
    }

    protected override void updateHeaderControl5(UnityEngine.Rect position)
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

    protected override void showBodyContextMenu(Event evt)
    {
        TimelineTrack itemTrack = TargetTrack.Behaviour as TimelineTrack;
        if (itemTrack == null) return;

        List<Type> trackTypes = itemTrack.GetAllowedCutsceneItems();
        GenericMenu createMenu = new GenericMenu();
        for (int i = 0; i < trackTypes.Count; i++)
        {
            ContextData data = getContextData(trackTypes[i]);
            data.Firetime = (evt.mousePosition.x - state.Translation.x) / state.Scale.x;
            createMenu.AddItem(new GUIContent(string.Format("Add New/{0}/{1}", data.Category, data.Label)), false, addCutsceneItem, data);
        }

        Behaviour b = DirectorCopyPaste.Peek();
        PasteContext pasteContext = new PasteContext(evt.mousePosition, itemTrack);
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

    private void showObjectPicker(ContextData data)
    {
        // Create an Object Picker with a dynamic type.
        MethodInfo method = typeof(EditorGUIUtility).GetMethod("ShowObjectPicker");
        MethodInfo generic = method.MakeGenericMethod(data.PairedType);
        generic.Invoke(this, new object[] { null, false, string.Empty, data.ControlID });

        savedData = data;
    }

    private void addCutsceneItem(object userData)
    {
        ContextData data = userData as ContextData;
        if (data != null)
        {
            if (data.PairedType == null)
            {
                GameObject item = CutsceneItemFactory.CreateCutsceneItem(data.Track, data.Type, data.Label, data.Firetime).gameObject;
                Undo.RegisterCreatedObjectUndo(item, string.Format("Create {0}", item.name));
            }
            else
            {
                showObjectPicker(data);
            }
        }
    }

    private void addCutsceneItem(ContextData data, UnityEngine.Object pickedObject)
    {
        GameObject item = CutsceneItemFactory.CreateCutsceneItem(data.Track, data.Type, data.Label, pickedObject, data.Firetime).gameObject;
        Undo.RegisterCreatedObjectUndo(item, string.Format("Create {0}", item.name));
    }

    private ContextData getContextData(Type type)
    {
        MemberInfo info = type;
        string label = string.Empty;
        string category = string.Empty;
        Type requiredObject = null;
        foreach (CutsceneItemAttribute attribute in info.GetCustomAttributes(typeof(CutsceneItemAttribute), true))
        {
            label = attribute.Label;
            category = attribute.Category;
            requiredObject = attribute.RequiredObjectType;
            break;
        }

        return new ContextData(this.controlID, type, requiredObject, (TargetTrack.Behaviour as TimelineTrack), category, label, state.ScrubberPosition);
    }

    private void pasteItem(object userData)
    {
        PasteContext data = userData as PasteContext;
        if (data != null)
        {
            float firetime = (data.mousePosition.x - state.Translation.x) / state.Scale.x;
            GameObject clone = DirectorCopyPaste.Paste(data.track.transform);

            clone.GetComponent<TimelineItem>().Firetime = firetime;

            Undo.RegisterCreatedObjectUndo(clone, "Pasted " + clone.name);
        }
    }

    private class ContextData
    {
        public int ControlID;
        public Type Type;
        public Type PairedType;
        public TimelineTrack Track;
        public string Category;
        public string Label;
        public float Firetime;

        public ContextData(int controlId, Type type, Type pairedType, TimelineTrack track, string category, string label, float firetime)
        {
            ControlID = controlId;
            Type = type;
            PairedType = pairedType;
            Track = track;
            Category = category;
            Label = label;
            Firetime = firetime;
        }
    }

    private class PasteContext
    {
        public Vector2 mousePosition;
        public TimelineTrack track;

        public PasteContext(Vector2 mousePosition, TimelineTrack track)
        {
            this.mousePosition = mousePosition;
            this.track = track;
        }
    }
}
