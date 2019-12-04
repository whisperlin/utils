
using System.Collections.Generic;
using UnityEngine;

namespace CinemaDirector
{
    /// <summary>
    /// Implement this interface in any track that is made for multiple actors.
    /// </summary>
    public interface IMultiActorTrack
    {
        /// <summary>
        /// Get the Actors associated with this Track.
        /// </summary>
        List<Transform> Actors
        {
            get;
        }
    }
}
