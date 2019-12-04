using System;
using System.Collections.Generic;
using UnityEngine;

namespace CinemaDirector
{
    public abstract class TimelineTrack : MonoBehaviour, IOptimizable
    {
        [SerializeField]
        public bool lockedStatus = false; //lockstate of track

        [SerializeField]
        private int ordinal = -1; // Ordering of Tracks

        [SerializeField]
        private bool canOptimize = true; // If true, this Track will load all items into cache on Optimize().

        // Options for when this Track will execute its Timeline Items.
        public PlaybackMode PlaybackMode = PlaybackMode.RuntimeAndEdit;

        protected float elapsedTime = 0f;

        // A cache of the TimelineItems for optimization purposes.
        protected TimelineItem[] itemCache;

        // A list of the cutscene item types that this Track is allowed to contain.
        protected List<Type> allowedItemTypes;

        private bool hasBeenOptimized = false;

        /// <summary>
        /// Prepares the TimelineTrack by caching all TimelineItems contained inside of it.
        /// </summary>
        public virtual void Optimize()
        {
            if (canOptimize)
            {
                itemCache = GetTimelineItems();
                hasBeenOptimized = true;
            }
            TimelineItem[] items = GetTimelineItems();
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] is IOptimizable)
                {
                    (items[i] as IOptimizable).Optimize();
                }
            }
        }

        /// <summary>
        /// Perform any initialization before the cutscene begins a fresh playback
        /// </summary>
        public virtual void Initialize()
        {
            elapsedTime = 0f;
            TimelineItem[] items = GetTimelineItems();
            for (int i = 0; i < items.Length; i++)
            {
                items[i].Initialize();
            }
        }

        /// <summary>
        /// Update the track to the given time
        /// </summary>
        /// <param name="time"></param>
        public virtual void UpdateTrack(float runningTime, float deltaTime)
        {
            float previousTime = elapsedTime;
            elapsedTime = runningTime;

            TimelineItem[] items = GetTimelineItems();
            for (int i = 0; i < items.Length; i++)
            {
                CinemaGlobalEvent cinemaEvent = items[i] as CinemaGlobalEvent;
                if (cinemaEvent == null) continue;
               
                if ((previousTime < cinemaEvent.Firetime || previousTime <= 0f) && elapsedTime >= cinemaEvent.Firetime)
                {
                    cinemaEvent.Trigger();
                }
                else if (previousTime >= cinemaEvent.Firetime && elapsedTime <= cinemaEvent.Firetime)
                {
                    cinemaEvent.Reverse();
                }
            }
            
            for (int i = 0; i < items.Length; i++)
            {
                CinemaGlobalAction action = items[i] as CinemaGlobalAction;
                if (action == null) continue;
                if (((previousTime < action.Firetime || previousTime <= 0f) && elapsedTime >= action.Firetime) && elapsedTime < action.EndTime)
                {
                    action.Trigger();
                }
                else if ((previousTime <= action.EndTime) && (elapsedTime >= action.EndTime))
                {
                    action.End();
                }
                else if (previousTime > action.Firetime && previousTime <= action.EndTime && elapsedTime <= action.Firetime)
                {
                    action.ReverseTrigger();
                }
                else if ((previousTime > action.EndTime || previousTime >= action.Cutscene.Duration) && (elapsedTime > action.Firetime) && (elapsedTime <= action.EndTime))
                {
                    action.ReverseEnd();
                }
                else if ((elapsedTime > action.Firetime) && (elapsedTime < action.EndTime))
                {
                    float t = runningTime - action.Firetime;
                    action.UpdateTime(t, deltaTime);
                }
            }
        }

        /// <summary>
        /// Notify track items that the cutscene has been paused
        /// </summary>
        public virtual void Pause() { }

        /// <summary>
        /// Notify track items that the cutscene has been resumed from a paused state.
        /// </summary>
        public virtual void Resume() { }

        /// <summary>
        /// The cutscene has been set to an arbitrary time by the user.
        /// Processing must take place to catch up to the new time.
        /// </summary>
        /// <param name="time">The new cutscene running time</param>
        public virtual void SetTime(float time)
        {
            float previousTime = elapsedTime;
            elapsedTime = time;

            TimelineItem[] items = GetTimelineItems();
            for (int i = 0; i < items.Length; i++)
            {
                // Check if it is a global event.
                CinemaGlobalEvent cinemaEvent = items[i] as CinemaGlobalEvent;
                if (cinemaEvent != null)
                {
                    if ((previousTime < cinemaEvent.Firetime && time >= cinemaEvent.Firetime) || (cinemaEvent.Firetime == 0f && previousTime <= cinemaEvent.Firetime && time > cinemaEvent.Firetime))
                    {
                        cinemaEvent.Trigger();
                    }
                    else if (previousTime > cinemaEvent.Firetime && elapsedTime <= cinemaEvent.Firetime)
                    {
                        cinemaEvent.Reverse();
                    }
                }

                // Check if it is a global action.
                CinemaGlobalAction action = items[i] as CinemaGlobalAction;
                if (action != null)
                {
                    action.SetTime((time - action.Firetime), time - previousTime);
                }
            }
        }

        /// <summary>
        /// Retrieve a list of important times for this track within the given range.
        /// </summary>
        /// <param name="from">The starting point of the range.</param>
        /// <param name="to">The end point of the range.</param>
        /// <returns>A list of ordered milestone times within the given range.</returns>
        public virtual List<float> GetMilestones(float from, float to)
        {
            bool isReverse = from > to;

            List<float> times = new List<float>();
            TimelineItem[] items = GetTimelineItems();
            for (int i = 0; i < items.Length; i++)
            {
                if ((!isReverse && from < items[i].Firetime && to >= items[i].Firetime) || (isReverse && from > items[i].Firetime && to <= items[i].Firetime))
                {
                    if (!times.Contains(items[i].Firetime))
                    {
                        times.Add(items[i].Firetime);
                    }
                }

                if (items[i] is TimelineAction)
                {
                    float endTime = (items[i] as TimelineAction).EndTime;
                    if ((!isReverse && from < endTime && to >= endTime) || (isReverse && from > endTime && to <= endTime))
                    {
                        if (!times.Contains(endTime))
                        {
                            times.Add(endTime);
                        }
                    }
                }
            }
            times.Sort();
            return times;
        }

        /// <summary>
        /// Notify the track items that the cutscene has been stopped
        /// </summary>
        public virtual void Stop()
        {
            elapsedTime = 0f;
            TimelineItem[] items = GetTimelineItems();
            for (int i = 0; i < items.Length; i++)
            {
                items[i].Stop();
            }
        }

        /// <summary>
        /// Returns all allowed Timeline Item types.
        /// </summary>
        /// <returns>A list of allowed cutscene item types.</returns>
        public List<Type> GetAllowedCutsceneItems()
        {
            if (allowedItemTypes == null)
            {
                allowedItemTypes = DirectorRuntimeHelper.GetAllowedItemTypes(this);
            }
            return allowedItemTypes;
        }

        /// <summary>
        /// The Cutscene that this Track is associated with. Can return null.
        /// </summary>
        public Cutscene Cutscene
        {
            get { return ((this.TrackGroup == null) ? null : this.TrackGroup.Cutscene); }
        }

        /// <summary>
        /// The TrackGroup associated with this Track. Can return null.
        /// </summary>
        public TrackGroup TrackGroup
        {
            get
            {
                TrackGroup group = null;
                if (transform.parent != null)
                {
                    // GetComponentInParent does not seem to work in Unity 5.3. 
                    // This means that the Cutscene hierarchy structure has to be strictly maintained.
#if UNITY_5_3_OR_NEWER
                    group = transform.parent.GetComponent<TrackGroup>();
#else
                    group = transform.parent.GetComponentInParent<TrackGroup>();
#endif
                    if (group == null)
                    {
                        Debug.LogError("No TrackGroup found on parent.", this);
                    }
                }
                else
                {
                    Debug.LogError("Track has no parent.", this);
                }
                return group;
            }
        }

        /// <summary>
        /// Ordinal for UI ranking.
        /// </summary>
        public int Ordinal
        {
            get
            {
                return ordinal;
            }
            set
            {
                ordinal = value;
            }
        }

        /// <summary>
        /// Enable this if the Track does not have Items added/removed during running.
        /// </summary>
        public bool CanOptimize
        {
            get { return canOptimize; }
            set { canOptimize = value; }
        }

        /// <summary>
        /// Get all TimelineItems that are allowed to be in this Track.
        /// </summary>
        /// <returns>A filtered list of Timeline Items.</returns>
        public TimelineItem[] GetTimelineItems()
        {
            // Return the cache if possible
            if (hasBeenOptimized)
            {
                return itemCache;
            }

            List<TimelineItem> items = new List<TimelineItem>();
            List<Type> allowedTypes = GetAllowedCutsceneItems();
            for (int i = 0; i < allowedTypes.Count; i++)
            {
                var components = GetComponentsInChildren(allowedTypes[i]);
                for (int j = 0; j < components.Length; j++)
                {
                    items.Add((TimelineItem)components[j]);
                }
            }
            return items.ToArray();
        }

        public virtual TimelineItem[] TimelineItems
        {
            get { return base.GetComponentsInChildren<TimelineItem>(); }
        }
    }

}