using System;
using System.Collections.Generic;
using UnityEngine;

namespace CinemaDirector
{
    /// <summary>
    /// The main organizational unit of a Cutscene, The TrackGroup contains tracks.
    /// </summary>
    [TrackGroupAttribute("Track Group", TimelineTrackGenre.GlobalTrack)]
    public abstract class TrackGroup : MonoBehaviour, IOptimizable
    {
        [SerializeField]
        private int ordinal = -1; // For ordering in UI

        [SerializeField]
        private bool canOptimize = true; // If true, this Track Group will load all tracks into cache on Optimize().






        // A cache of the tracks for optimization purposes.
        protected TimelineTrack[] trackCache;

        // A list of the types that this Track Group is allowed to contain.
        protected List<Type> allowedTrackTypes;

        private bool hasBeenOptimized = false;

        /// <summary>
        /// Prepares the TrackGroup by caching all TimelineTracks.
        /// </summary>
        public virtual void Optimize()
        {
            if (canOptimize)
            {
                trackCache = GetTracks();
                hasBeenOptimized = true;
            }
            TimelineTrack[] tracks = GetTracks();
            for (int i = 0; i < tracks.Length; i++)
            {
                tracks[i].Optimize();
            }
        }

        /// <summary>
        /// Initialize all Tracks before beginning a fresh playback.
        /// </summary>
        public virtual void Initialize()
        {
            TimelineTrack[] tracks = GetTracks();
            for (int i = 0; i < tracks.Length; i++)
            {
                tracks[i].Initialize();
            }
        }

        /// <summary>
        /// Update the track group to the current running time of the cutscene.
        /// </summary>
        /// <param name="time">The current running time</param>
        /// <param name="deltaTime">The deltaTime since the last update call</param>
        public virtual void UpdateTrackGroup(float time, float deltaTime)
        {
            TimelineTrack[] tracks = GetTracks();
            for (int i = 0; i < tracks.Length; i++)
            {
                tracks[i].UpdateTrack(time, deltaTime);
            }
        }

        /// <summary>
        /// Pause all Track Items that this TrackGroup contains.
        /// </summary>
        public virtual void Pause()
        {
            TimelineTrack[] tracks = GetTracks();
            for (int i = 0; i < tracks.Length; i++)
            {
                tracks[i].Pause();
            }
        }

        /// <summary>
        /// Stop all Track Items that this TrackGroup contains.
        /// </summary>
        public virtual void Stop()
        {
            TimelineTrack[] tracks = GetTracks();
            for (int i = 0; i < tracks.Length; i++)
            {
                tracks[i].Stop();
            }
        }

        /// <summary>
        /// Resume all Track Items that this TrackGroup contains.
        /// </summary>
        public virtual void Resume()
        {
            TimelineTrack[] tracks = GetTracks();
            for (int i = 0; i < tracks.Length; i++)
            {
                tracks[i].Resume();
            }
        }

        /// <summary>
        /// Set this TrackGroup to the state of a given new running time.
        /// </summary>
        /// <param name="time">The new running time</param>
        public virtual void SetRunningTime(float time)
        {
            TimelineTrack[] tracks = GetTracks();
            for (int i = 0; i < tracks.Length; i++)
            {
                tracks[i].SetTime(time);
            }
        }

        /// <summary>
        /// Retrieve a list of important times for this track group within the given range.
        /// </summary>
        /// <param name="from">the starting time</param>
        /// <param name="to">the ending time</param>
        /// <returns>A list of ordered milestone times within the given range.</returns>
        public virtual List<float> GetMilestones(float from, float to)
        {
            List<float> times = new List<float>();
            TimelineTrack[] tracks = GetTracks();
            for (int i = 0; i < tracks.Length; i++)
            {
                List<float> trackTimes = tracks[i].GetMilestones(from, to);
                for (int j = 0; j < trackTimes.Count; j++)
                {
                    if(!times.Contains(trackTimes[j]))
                    {
                        times.Add(trackTimes[j]);
                    }
                }
            }
            times.Sort();
            return times;
        }

        /// <summary>
        /// The Cutscene that this TrackGroup is associated with. Will return null if TrackGroup does not have a Cutscene as a parent.
        /// </summary>
        public Cutscene Cutscene
        {
            get
            {
                Cutscene cutscene = null;
                if (transform.parent != null)
                {
                    cutscene = transform.parent.GetComponentInParent<Cutscene>();
                    if (cutscene == null)
                    {
                        Debug.LogError("No Cutscene found on parent!", this);
                    }
                }
                else
                {
                    Debug.LogError("TrackGroup has no parent!", this);
                }
                return cutscene;
            }
        }

        /// <summary>
        /// The TimelineTracks that this TrackGroup contains.
        /// </summary>
        public virtual TimelineTrack[] GetTracks()
        {
            // Return the cache if possible
            if (hasBeenOptimized)
            {
                return trackCache;
            }

            List<TimelineTrack> tracks = new List<TimelineTrack>();
            List<Type> allowedTypes = GetAllowedTrackTypes();
            for (int i = 0; i < allowedTypes.Count; i++)
            {
                var components = GetComponentsInChildren(allowedTypes[i]);
                for (int j = 0; j < components.Length; j++)
                {
                    tracks.Add((TimelineTrack)components[j]);
                }
            }

            tracks.Sort(
                delegate(TimelineTrack track1, TimelineTrack track2)
                { 
                    return track1.Ordinal - track2.Ordinal;
                });
            return tracks.ToArray();
        }

        /// <summary>
        /// Provides a list of Types this Track Group is allowed to contain. Loaded by looking at Attributes.
        /// </summary>
        /// <returns>The list of track types.</returns>
        public List<Type> GetAllowedTrackTypes()
        {
            if (allowedTrackTypes == null)
            {
                allowedTrackTypes = DirectorRuntimeHelper.GetAllowedTrackTypes(this);
            }
            return allowedTrackTypes;
        }

        /// <summary>
        /// Ordinal for UI ranking.
        /// </summary>
        public int Ordinal
        {
            get { return ordinal; }
            set { ordinal = value; }
        }

        /// <summary>
        /// Enable this if the TrackGroup does not have Tracks added/removed during running.
        /// </summary>
        public bool CanOptimize
        {
            get { return canOptimize; }
            set { canOptimize = value; }
        }
    }
}