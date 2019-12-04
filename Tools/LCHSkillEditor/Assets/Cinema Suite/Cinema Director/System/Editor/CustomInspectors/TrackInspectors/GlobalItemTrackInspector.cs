using System;
using UnityEditor;
using UnityEngine;
using CinemaDirector;

[CustomEditor(typeof(GlobalItemTrack))]
public class GlobalItemTrackInspector : Editor
{
    // Properties
    private SerializedObject eventTrack;
    private bool actionFoldout = true;

    GUIContent addActionContent = new GUIContent("Add New Action", "Add a new action to this track.");
    GUIContent actionContent = new GUIContent("Actions", "The actions associated with this track.");
    /// <summary>
    /// On inspector enable, load serialized objects
    /// </summary>
    public void OnEnable()
    {
        eventTrack = new SerializedObject(this.target);
    }

    /// <summary>
    /// Update and Draw the inspector
    /// </summary>
    public override void OnInspectorGUI()
    {
        eventTrack.Update();
        GlobalItemTrack track = base.serializedObject.targetObject as GlobalItemTrack;

        CinemaGlobalAction[] actions = track.Actions;
        CinemaGlobalEvent[] events = track.Events;

        if (actions.Length > 0 || events.Length > 0)
        {
            actionFoldout = EditorGUILayout.Foldout(actionFoldout, actionContent);
            if (actionFoldout)
            {
                EditorGUI.indentLevel++;

                for (int i = 0; i < actions.Length; i++)
                {
                    EditorGUILayout.ObjectField(actions[i].name, actions[i], typeof(CinemaGlobalAction), true);
                }
                for (int i = 0; i < events.Length; i++)
                {
                    EditorGUILayout.ObjectField(events[i].name, events[i], typeof(CinemaGlobalEvent), true);
                }
                EditorGUI.indentLevel--;
            }
        }

        if (GUILayout.Button(addActionContent))
        {
            GenericMenu createMenu = new GenericMenu();

            Type[] actionSubTypes = DirectorHelper.GetAllSubTypes(typeof(CinemaGlobalAction));
            for (int i = 0; i < actionSubTypes.Length; i++)
            {
                string text = string.Empty;
                string category = string.Empty;
                string label = string.Empty;
                object[] attrs = actionSubTypes[i].GetCustomAttributes(typeof(CutsceneItemAttribute), true);
                for (int j = 0; j < attrs.Length; j++)
                {
                    CutsceneItemAttribute attribute = attrs[j] as CutsceneItemAttribute;
                    if (attribute != null)
                    {
                        category = attribute.Category;
                        label = attribute.Label;
                        text = string.Format("{0}/{1}", attribute.Category, attribute.Label);
                        break;
                    }
                }
                if (label != string.Empty)
                {
                    ContextData userData = new ContextData { Type = actionSubTypes[i], Label = label, Category = category };
                    createMenu.AddItem(new GUIContent(text), false, new GenericMenu.MenuFunction2(AddEvent), userData);
                }
            }

            Type[] eventSubTypes = DirectorHelper.GetAllSubTypes(typeof(CinemaGlobalEvent));
            for (int i = 0; i < eventSubTypes.Length; i++)
            {
                string text = string.Empty;
                string category = string.Empty;
                string label = string.Empty;
                object[] attrs = eventSubTypes[i].GetCustomAttributes(typeof(CutsceneItemAttribute), true);
                for (int j = 0; j < attrs.Length; j++)
                {
                    CutsceneItemAttribute attribute = attrs[j] as CutsceneItemAttribute;
                    if (attribute != null)
                    {
                        category = attribute.Category;
                        label = attribute.Label;
                        text = string.Format("{0}/{1}", attribute.Category, attribute.Label);
                        break;
                    }
                }
                if (label != string.Empty)
                {
                    ContextData userData = new ContextData { Type = eventSubTypes[i], Label = label, Category = category };
                    createMenu.AddItem(new GUIContent(text), false, new GenericMenu.MenuFunction2(AddEvent), userData);
                }
            }
            createMenu.ShowAsContext();
        }

        eventTrack.ApplyModifiedProperties();
    }

    private void AddEvent(object userData)
    {
        ContextData data = userData as ContextData;
        if (data != null)
        {
            string name = DirectorHelper.getCutsceneItemName(data.Label, data.Type);
            GameObject trackEvent = new GameObject(name);
            trackEvent.AddComponent(data.Type);
            trackEvent.transform.parent = (this.target as GlobalItemTrack).transform;
        }
    }

    private class ContextData
    {
        public Type Type;
        public string Category;
        public string Label;
    }
}
