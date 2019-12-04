using System.Collections.Generic;
// Cinema Suite
using UnityEngine;

namespace CinemaDirector
{
    /// <summary>
    /// An implementation of an event that can be performed on an arbitrary actor.
    /// </summary>
    [ExecuteInEditMode]
    public abstract class CinemaActorEvent : TimelineItem
    {
        /// <summary>
        /// Called when the running time of the cutscene hits the firetime of the event.
        /// </summary>
        /// <param name="Actor">The actor to perform the event on.</param>
        /// <remarks>
        /// <c>Trigger()</c> is only called when the cutscene hits the firetime while moving forward, either through regular playback or by scrubbing. For when the playback is reversed, see <see cref="Reverse"/>.
        /// </remarks>
        public abstract void Trigger(GameObject Actor);

        /// <summary>
        /// Reverse trigger. Called when scrubbing backwards past the firetime of the event.
        /// </summary>
        /// <param name="Actor">The actor to perform the event on.</param>
        public virtual void Reverse(GameObject Actor) { }

        public virtual void SetTimeTo(float deltaTime) { }

        /// <summary>
        /// Pause any action as necessary. Called when the cutscene is paused.
        /// </summary>
        public virtual void Pause() { }

        /// <summary>
        /// Resume from paused. Called when the cutscene is unpaused.
        /// </summary>
        public virtual void Resume() { }

        /// <summary>
        /// Initialize the actor. Called before entering preview mode.
        /// </summary>
        /// <param name="Actor">The actor to initialize.</param>
        public virtual void Initialize(GameObject Actor) { }

        /// <summary>
        /// Called when the cutscene exits preview/play mode. Return properties to pre-cached state if necessary.
        /// </summary>
        /// <param name="Actor">The actor to target for this event.</param>
        public virtual void Stop(GameObject Actor) { }

        /// <summary>
        /// Get the actors associated with this Actor Event. Can return null.
        /// </summary>
        /// <returns>A set of actors related to this actor event.</returns>
        public virtual List<Transform> GetActors()
        {
            IMultiActorTrack track = (TimelineTrack as IMultiActorTrack);
            if (track != null)
            {
                return track.Actors;
            }
            return null;
        }

        /// <summary>
        /// The Actor Track Group associated with this event.
        /// </summary>
        public ActorTrackGroup ActorTrackGroup
        {
            get
            {
                return this.TimelineTrack.TrackGroup as ActorTrackGroup;
            }
        }

        
    }
}