using CinemaDirector;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

/// <summary>
/// A utility class for cutscene
/// </summary>
public class DirectorHelper
{
    public static CutsceneWrapper UpdateWrapper(Cutscene cutscene, CutsceneWrapper wrapper)
    {
        if (cutscene == null) return null;

        if (wrapper == null || !cutscene.Equals(wrapper.Behaviour))
        {
            return CreateWrapper(cutscene);
        }
        else
        {
            wrapper.Behaviour = cutscene;
            wrapper.Duration = cutscene.Duration;
            wrapper.IsPlaying = cutscene.State == Cutscene.CutsceneState.PreviewPlaying || cutscene.State == Cutscene.CutsceneState.Playing;
            wrapper.RunningTime = cutscene.RunningTime;

            List<Behaviour> itemsToRemove = new List<Behaviour>();
            foreach (Behaviour behaviour in wrapper.Behaviours)
            {
                bool found = false;
                foreach (TrackGroup group in cutscene.TrackGroups)
                {
                    if (behaviour.Equals(group))
                    {
                        found = true;
                        break;
                    }
                }
                if (!found || behaviour == null)
                {
                    itemsToRemove.Add(behaviour);
                }
            }

            foreach (Behaviour trackGroup in itemsToRemove)
            {
                wrapper.HasChanged = true;
                wrapper.RemoveTrackGroup(trackGroup);
            }

            foreach (TrackGroup tg in cutscene.TrackGroups)
            {
                TrackGroupWrapper tgWrapper = null;
                if (!wrapper.ContainsTrackGroup(tg, out tgWrapper))
                {
                    
                    tgWrapper = new TrackGroupWrapper(tg);
                    tgWrapper.Ordinal = tg.Ordinal;
                    wrapper.AddTrackGroup(tg, tgWrapper);
                    wrapper.HasChanged = true;
                }

                foreach (TimelineTrack track in tg.GetTracks())
                {
                    TimelineTrackWrapper trackWrapper = null;
                    if (!tgWrapper.ContainsTrack(track, out trackWrapper))
                    {
                        trackWrapper = new TimelineTrackWrapper(track);
                        trackWrapper.Ordinal = track.Ordinal;
                        tgWrapper.AddTrack(track, trackWrapper);
                        tgWrapper.HasChanged = true;
                    }

                    foreach (TimelineItem item in track.GetTimelineItems())
                    {
                        TimelineItemWrapper itemWrapper = null;
                        if (!trackWrapper.ContainsItem(item, out itemWrapper))
                        {
                            if (item.GetType().IsSubclassOf(typeof(CinemaClipCurve)))
                            {
                                CinemaClipCurve clip = item as CinemaClipCurve;
                                itemWrapper = new CinemaClipCurveWrapper(clip, clip.Firetime, clip.Duration);
                                trackWrapper.AddItem(clip, itemWrapper);
                            }
                            else if (item.GetType().IsSubclassOf(typeof(CinemaTween)))
                            {
                                CinemaTween tween = item as CinemaTween;
                                itemWrapper = new CinemaTweenWrapper(tween, tween.Firetime, tween.Duration);
                                trackWrapper.AddItem(tween, itemWrapper);
                            }
                            else if (item.GetType().IsSubclassOf(typeof(TimelineActionFixed)))
                            {
                                TimelineActionFixed fixedAction = item as TimelineActionFixed;
                                itemWrapper = new CinemaActionFixedWrapper(fixedAction, fixedAction.Firetime, fixedAction.Duration, fixedAction.InTime, fixedAction.OutTime, fixedAction.ItemLength);
                                trackWrapper.AddItem(fixedAction, itemWrapper);
                            }
                            else if (item.GetType().IsSubclassOf(typeof(TimelineAction)))
                            {
                                TimelineAction action = item as TimelineAction;
                                itemWrapper = new CinemaActionWrapper(action, action.Firetime, action.Duration);
                                trackWrapper.AddItem(action, itemWrapper);
                            }
                            else
                            {
                                itemWrapper = new TimelineItemWrapper(item, item.Firetime);
                                trackWrapper.AddItem(item, itemWrapper);
                            }
                            trackWrapper.HasChanged = true;
                        }
                        else
                        {
                            if (GUIUtility.hotControl == 0)
                            {
                                if (itemWrapper.GetType() == (typeof(CinemaClipCurveWrapper)))
                                {
                                    CinemaClipCurve clip = item as CinemaClipCurve;
                                    CinemaClipCurveWrapper clipWrapper = itemWrapper as CinemaClipCurveWrapper;
                                    clipWrapper.Firetime = clip.Firetime;
                                    clipWrapper.Duration = clip.Duration;
                                }
                                else if (itemWrapper.GetType() == (typeof(CinemaTweenWrapper)))
                                {
                                }
                                else if (itemWrapper.GetType() == (typeof(CinemaActionFixedWrapper)))
                                {
                                    TimelineActionFixed actionFixed = item as TimelineActionFixed;
                                    CinemaActionFixedWrapper actionFixedWrapper = itemWrapper as CinemaActionFixedWrapper;
                                    actionFixedWrapper.Firetime = actionFixed.Firetime;
                                    actionFixedWrapper.Duration = actionFixed.Duration;
                                    actionFixedWrapper.InTime = actionFixed.InTime;
                                    actionFixedWrapper.OutTime = actionFixed.OutTime;
                                    actionFixedWrapper.ItemLength = actionFixed.ItemLength;
                                }
                                else if (itemWrapper.GetType() == (typeof(CinemaActionWrapper)))
                                {
                                    TimelineAction action = item as TimelineAction;
                                    CinemaActionWrapper actionWrapper = itemWrapper as CinemaActionWrapper;
                                    actionWrapper.Firetime = action.Firetime;
                                    actionWrapper.Duration = action.Duration;
                                }
                                else
                                {
                                    itemWrapper.Firetime = item.Firetime;
                                }
                            }
                        }
                    }

                    // Remove missing track items
                    List<Behaviour> itemRemovals = new List<Behaviour>();
                    foreach (Behaviour behaviour in trackWrapper.Behaviours)
                    {
                        bool found = false;
                        foreach (TimelineItem item in track.GetTimelineItems())
                        {
                            if (behaviour.Equals(item))
                            {
                                found = true;
                                break;
                            }
                        }
                        if (!found || behaviour == null)
                        {
                            itemRemovals.Add(behaviour);
                        }
                    }
                    foreach (Behaviour item in itemRemovals)
                    {
                        trackWrapper.HasChanged = true;
                        trackWrapper.RemoveItem(item);
                    }
                    trackWrapper.IsLocked = track.lockedStatus;
                }
                
                // Remove missing tracks
                List<Behaviour> removals = new List<Behaviour>();
                foreach (Behaviour behaviour in tgWrapper.Behaviours)
                {
                    bool found = false;
                    foreach (TimelineTrack track in tg.GetTracks())
                    {
                        if (behaviour.Equals(track))
                        {
                            found = true;
                            break;
                        }
                    }
                    if (!found || behaviour == null)
                    {
                        removals.Add(behaviour);
                    }
                }
                foreach (Behaviour track in removals)
                {
                    tgWrapper.HasChanged = true;
                    tgWrapper.RemoveTrack(track);
                }
            }
        }

        return wrapper;
    }

    public static void ReflectChanges(Cutscene cutscene, CutsceneWrapper wrapper)
    {
        if (cutscene == null || wrapper == null) return;
        
        cutscene.Duration = wrapper.Duration;
        foreach (TrackGroupWrapper tgw in wrapper.TrackGroups)
        {
            TrackGroup tg = tgw.Behaviour as TrackGroup;
            tg.Ordinal = tgw.Ordinal;

            foreach (TimelineTrackWrapper trackWrapper in tgw.Tracks)
            {
                TimelineTrack track = trackWrapper.Behaviour as TimelineTrack;
                track.Ordinal = trackWrapper.Ordinal;
            }
        }
    }

    public static CutsceneWrapper CreateWrapper(Cutscene cutscene)
    {
        CutsceneWrapper wrapper = new CutsceneWrapper(cutscene);
        if (cutscene != null)
        {
            wrapper.RunningTime = cutscene.RunningTime;
            wrapper.Duration = cutscene.Duration;
            wrapper.IsPlaying = cutscene.State == Cutscene.CutsceneState.PreviewPlaying || cutscene.State == Cutscene.CutsceneState.Playing;
           
            foreach (TrackGroup tg in cutscene.TrackGroups)
            {
                TrackGroupWrapper tgWrapper = new TrackGroupWrapper(tg);
                tgWrapper.Ordinal = tg.Ordinal;
                wrapper.AddTrackGroup(tg, tgWrapper);

                foreach (TimelineTrack track in tg.GetTracks())
                {
                    TimelineTrackWrapper trackWrapper = new TimelineTrackWrapper(track);
                    trackWrapper.Ordinal = track.Ordinal;
                    tgWrapper.AddTrack(track, trackWrapper);

                    foreach (TimelineItem item in track.GetTimelineItems())
                    {
                        if (item.GetType().IsSubclassOf(typeof(CinemaClipCurve)))
                        {
                            CinemaClipCurve clip = item as CinemaClipCurve;
                            CinemaClipCurveWrapper clipWrapper = new CinemaClipCurveWrapper(clip, clip.Firetime, clip.Duration);
                            trackWrapper.AddItem(clip, clipWrapper);
                        }
                        else if (item.GetType().IsSubclassOf(typeof(CinemaTween)))
                        {
                        }
                        else if (item.GetType().IsSubclassOf(typeof(TimelineActionFixed)))
                        {
                            TimelineActionFixed actionFixed = item as TimelineActionFixed;
                            CinemaActionFixedWrapper actionFixedWrapper = new CinemaActionFixedWrapper(actionFixed, actionFixed.Firetime, actionFixed.Duration, actionFixed.InTime, actionFixed.OutTime, actionFixed.ItemLength);
                            trackWrapper.AddItem(actionFixed, actionFixedWrapper);
                        }
                        else if (item.GetType().IsSubclassOf(typeof(TimelineAction)))
                        {
                            TimelineAction action = item as TimelineAction;
                            CinemaActionWrapper itemWrapper = new CinemaActionWrapper(action, action.Firetime, action.Duration);
                            trackWrapper.AddItem(action, itemWrapper);
                        }
                        else
                        {
                            TimelineItemWrapper itemWrapper = new TimelineItemWrapper(item, item.Firetime);
                            trackWrapper.AddItem(item, itemWrapper);
                        }
                    }
                }
            }
        }
        return wrapper;
    }

    public static System.Type[] GetAllSubTypes(System.Type ParentType)
    {
        List<System.Type> list = new List<System.Type>();
        foreach (Assembly a in System.AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (System.Type type in a.GetTypes())
            {
                if (type.IsSubclassOf(ParentType))
                {
                    list.Add(type);
                }
            }
        }
        return list.ToArray();
    }

    public enum TimeEnum
    {
        Minutes = 60,
        Seconds = 1
    }

    /// <summary>
    /// Determines if a Cutscene exists with a given name. 
    /// This is to avoid cutscenes with the same name, but is not enforced by Unity.
    /// </summary>
    /// <param name="name">The name to check</param>
    /// <returns>true if the name exists</returns>
    public static bool isCutsceneNameDuplicate(string name)
    {
        bool isDuplicate = false;
        Object[] cutscenes = Object.FindObjectsOfType(typeof(Cutscene));
        foreach (Object obj in cutscenes)
        {
            if (name == obj.name)
            {
                isDuplicate = true;
            }
        }

        return isDuplicate;
    }

    public static string getCutsceneItemName(GameObject parent, string name, System.Type type)
    {
        Transform[] children = parent.GetComponentsInChildren<Transform>();
        return getCutsceneItemName(children, name, type, 0);
    }

    public static string getCutsceneItemName(string name, System.Type type)
    {
        return getCutsceneItemName(name, type, 0);
    }
  
    private static string getCutsceneItemName(string name, System.Type type, int iteration)
    {
        string newName = name;
        if (iteration > 0)
        {
            newName = string.Format("{0} {1}", name, iteration);
        }
        bool isDuplicate = false;
        Object[] items = Object.FindObjectsOfType(type);
        foreach (Object obj in items)
        {
            if (newName == obj.name)
            {
                isDuplicate = true;
            }
        }

        if (isDuplicate)
        {
            return getCutsceneItemName(name, type, ++iteration);
        }
        return newName;
    }

    private static string getCutsceneItemName(Transform[] children, string name, System.Type type, int iteration)
    {
        string newName = name;
        if (iteration > 0)
        {
            newName = string.Format("{0} {1}", name, iteration);
        }
        bool isDuplicate = false;
        foreach (Object obj in children)
        {
            if (newName == obj.name)
            {
                isDuplicate = true;
            }
        }

        if (isDuplicate)
        {
            return getCutsceneItemName(children, name, type, ++iteration);
        }
        return newName;
    }

    public static Component[] getValidComponents(GameObject actor)
    {
        Component[] components = actor.GetComponents<Component>();
        List<Component> validComponents = new List<Component>();
        for (int i = 0; i < components.Length; i++)
        {
            if (components[i] != null)
                validComponents.Add(components[i]);
        }

        return validComponents.ToArray();
    }

    public static Component[] getEnableableComponents(GameObject actor)
    {
        List<Component> behaviours = new List<Component>();
        foreach(Component c in actor.GetComponents<Component>())
        {
            foreach (MemberInfo field in c.GetType().GetMember("enabled"))
            {
                if (field.Name == "enabled")
                {
                    behaviours.Add(c);
                    continue;
                }
            }
        }
        return behaviours.ToArray();
    }

    public static PropertyInfo[] getValidProperties(Component component)
    {
        List<PropertyInfo> properties = new List<PropertyInfo>();
        foreach (PropertyInfo propertyInfo in component.GetType().GetProperties())
        {
            if (UnityPropertyTypeInfo.GetMappedType(propertyInfo.PropertyType) != PropertyTypeInfo.None && propertyInfo.CanWrite)
            {
                properties.Add(propertyInfo);
            }
        }
        return properties.ToArray();
    }

    public static FieldInfo[] getValidFields(Component component)
    {
        List<FieldInfo> fields = new List<FieldInfo>();
        foreach(FieldInfo field in component.GetType().GetFields())
        {
            if (UnityPropertyTypeInfo.GetMappedType(field.FieldType) != PropertyTypeInfo.None)
            {
                fields.Add(field);
            }
        }
        return fields.ToArray();
    }

    public static MemberInfo[] getValidMembers(Component component)
    {
        PropertyInfo[] properties = getValidProperties(component);
        FieldInfo[] fields = getValidFields(component);

        List<MemberInfo> members = new List<MemberInfo>();
        if(component.GetType() == typeof(Transform))
        {
            foreach (PropertyInfo propertyInfo in properties)
            {
                if (propertyInfo.Name == "localPosition" || propertyInfo.Name == "localEulerAngles" || propertyInfo.Name == "localScale")
                {
                    members.Add(propertyInfo);
                }
            }
        }
        else
        {
            members.AddRange(properties);
            members.AddRange(fields);
        }
        return members.ToArray();
    }

    public static string GetUserFriendlyName(Component component, MemberInfo memberInfo)
    {
        return GetUserFriendlyName(component.GetType().Name, memberInfo.Name);
    }

    public static string GetUserFriendlyName(string componentName, string memberName)
    {
        string name = memberName;
        if (componentName == "Transform")
        {
            if (memberName == "localPosition")
            {
                name = "Position";
            }
            else if (memberName == "localEulerAngles")
            {
                name = "Rotation";
            }
            else if (memberName == "localScale")
            {
                name = "Scale";
            }
        }
        else
        {
            //'camelCase' to 'Title Case'
            const string pattern = @"(?<=[^A-Z])(?=[A-Z])";
            name = Regex.Replace(memberName, pattern, " ", RegexOptions.None);
            name = name.Substring(0, 1).ToUpper() + name.Substring(1);
        }
        return name;
    }

    public static bool IsTrackItemValidForTrack(Behaviour behaviour, TimelineTrack track)
    {
        bool retVal = false;
        if (track.GetType()==(typeof(ShotTrack)))
        {
            if (behaviour.GetType() == (typeof(CinemaShot)))
            {
                retVal = true;
            }
        }
        else if (track.GetType()==(typeof(AudioTrack)))
        {
            if (behaviour.GetType() == (typeof(CinemaAudio)))
            {
                retVal = true;
            }
        }
        else if (track.GetType()==(typeof(GlobalItemTrack)) || track.GetType().IsSubclassOf(typeof(GlobalItemTrack)))
        {
            if (behaviour.GetType() == typeof(CinemaShot) || (behaviour.GetType().IsSubclassOf(typeof(CinemaAudio))))
            {
                retVal = false;
            }
            else if (behaviour.GetType().IsSubclassOf(typeof(CinemaGlobalAction)) || behaviour.GetType().IsSubclassOf(typeof(CinemaGlobalEvent)))
            {
                retVal = true;
            }
        }
        else if (track.GetType()==(typeof(ActorItemTrack)) || track.GetType().IsSubclassOf(typeof(ActorItemTrack)))
        {
            if (behaviour.GetType().IsSubclassOf(typeof(CinemaActorAction)) || behaviour.GetType().IsSubclassOf(typeof(CinemaActorEvent)))
            {
                retVal = true;
            }
        }
        else if (track.GetType() == (typeof(CurveTrack)) || track.GetType().IsSubclassOf(typeof(CurveTrack)))
        {
            if (behaviour.GetType()==(typeof(CinemaActorClipCurve)))
            {
                retVal = true;
            }
        }
        else if (track.GetType() == (typeof(MultiCurveTrack)) || track.GetType().IsSubclassOf(typeof(MultiCurveTrack)))
        {
            if (behaviour.GetType()==(typeof(CinemaMultiActorCurveClip)))
            {
                retVal = true;
            }
        }
        return retVal;
    }
}
