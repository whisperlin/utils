using System.Collections.Generic;
using UnityEngine;

namespace CinemaDirector
{
    /// <summary>
    /// The MultiActorTrackGroup maintains a list of Actors that have something in 
    /// common and a set of tracks that act upon those Actors.
    /// </summary>
    [TrackGroupAttribute("MultiActor Track Group", TimelineTrackGenre.MultiActorTrack)]
    public class MultiActorTrackGroup : TrackGroup
    {
        [SerializeField]
        private List<Transform> actors = new List<Transform>();

        /// <summary>
        /// The Actors that this TrackGroup is focused on
        /// </summary>
        public List<Transform> Actors
        {
            get
            {
                return actors;
            }
            set
            {
                actors = value;
            }
        }
    }
}
