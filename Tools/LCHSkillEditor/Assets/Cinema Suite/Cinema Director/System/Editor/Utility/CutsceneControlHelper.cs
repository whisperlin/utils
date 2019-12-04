using CinemaDirector;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace CinemaDirectorControl.Utility
{
    /// <summary>
    /// Helper class for Cutscene controls.
    /// </summary>
    public static class CutsceneControlHelper
    {
        /// <summary>
        /// Show the "Add Track Group" context menu.
        /// </summary>
        /// <param name="cutscene">The current Cutscene that the track group will be added to.</param>
        public static void ShowAddTrackGroupContextMenu(Cutscene cutscene)
        {
            GenericMenu createMenu = new GenericMenu();
            Type[] subTypes = DirectorHelper.GetAllSubTypes(typeof(TrackGroup));
            for (int i = 0; i < subTypes.Length; i++)
            {
                TrackGroupContextData userData = getContextDataFromType(cutscene, subTypes[i]);
                string text = string.Format(userData.Label);
                createMenu.AddItem(new GUIContent(text), false, new GenericMenu.MenuFunction2(AddTrackGroup), userData);
            }

            createMenu.ShowAsContext();
        }

        public static void ShowAddTrackContextMenu(TrackGroup trackGroup)
        {
            List<Type> trackTypes = trackGroup.GetAllowedTrackTypes();

            GenericMenu createMenu = new GenericMenu();

            // Get the attributes of each track.
            for (int i = 0; i < trackTypes.Count; i++)
            {
                MemberInfo info = trackTypes[i];
                string label = string.Empty;
                foreach (TimelineTrackAttribute attribute in info.GetCustomAttributes(typeof(TimelineTrackAttribute), true))
                {
                    label = attribute.Label;
                    break;
                }

                createMenu.AddItem(new GUIContent(string.Format("Add {0}", label)), false, addTrack, new TrackContextData(label, trackTypes[i], trackGroup));
            }

            createMenu.ShowAsContext();
        }

        private static void AddTrackGroup(object userData)
        {
            TrackGroupContextData data = userData as TrackGroupContextData;
            if (data != null)
            {
                GameObject item = CutsceneItemFactory.CreateTrackGroup(data.Cutscene, data.Type, data.Label).gameObject;
                Undo.RegisterCreatedObjectUndo(item, string.Format("Create {0}", item.name));
            }
        }

        private static TrackGroupContextData getContextDataFromType(Cutscene cutscene, Type type)
        {
            string label = string.Empty;
            object[] attrs = type.GetCustomAttributes(typeof(TrackGroupAttribute), true);
            for (int i = 0; i < attrs.Length; i++)
            {
                TrackGroupAttribute attribute = attrs[i] as TrackGroupAttribute;
                if (attribute != null)
                {
                    label = attribute.Label;
                    break;
                }
            }
            TrackGroupContextData userData = new TrackGroupContextData { Cutscene = cutscene, Type = type, Label = label };
            return userData;
        }

        private class TrackGroupContextData
        {
            public Cutscene Cutscene;
            public Type Type;
            public string Label;
        }

        /// <summary>
        /// Add a new track
        /// </summary>
        /// <param name="userData">TrackContextData for the track to be created.</param>
        private static void addTrack(object userData)
        {
            TrackContextData trackData = userData as TrackContextData;
            if (trackData != null)
            {
                GameObject item = CutsceneItemFactory.CreateTimelineTrack(trackData.TrackGroup, trackData.Type, trackData.Label).gameObject;
                Undo.RegisterCreatedObjectUndo(item, string.Format("Create {0}", item.name));
            }
        }

        /// <summary>
        /// Context data for the track to be created.
        /// </summary>
        private class TrackContextData
        {
            public string Label;
            public Type Type;
            public TrackGroup TrackGroup;

            public TrackContextData(string label, Type type, TrackGroup trackGroup)
            {
                this.Label = label;
                this.Type = type;
                this.TrackGroup = trackGroup;
            }
        }
        
    }
}
