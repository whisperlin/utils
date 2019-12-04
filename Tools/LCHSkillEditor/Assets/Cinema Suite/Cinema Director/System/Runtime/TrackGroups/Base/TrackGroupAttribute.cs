// Cinema Suite 2014
using System;
using System.Collections.Generic;

namespace CinemaDirector
{
    /// <summary>
    /// The Attribute for track groups.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class TrackGroupAttribute : Attribute
    {
        // The user friendly name for this Track Group.
        private string label; 

        // The list of Track Genres that this Track Group allows.
        private List<TimelineTrackGenre> trackGenres = new List<TimelineTrackGenre>();

        /// <summary>
        /// Attribute for Track Groups
        /// </summary>
        /// <param name="label">The name of this track group.</param>
        /// <param name="TrackGenres">The Track Genres that this Track Group is allowed to contain.</param>
        public TrackGroupAttribute(string label, params TimelineTrackGenre[] TrackGenres)
        {
            this.label = label;
            this.trackGenres.AddRange(TrackGenres);
        }

        /// <summary>
        /// The label of this track group.
        /// </summary>
        public string Label
        {
            get
            {
                return label;
            }
        }

        /// <summary>
        /// The Track Genres that this Track Group can contain.
        /// </summary>
        public TimelineTrackGenre[] AllowedTrackGenres
        {
            get
            {
                return trackGenres.ToArray();
            }
        }
    }
}