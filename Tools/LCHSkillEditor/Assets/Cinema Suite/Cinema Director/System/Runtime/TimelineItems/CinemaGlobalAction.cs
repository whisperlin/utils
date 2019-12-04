using System;
using UnityEngine;

namespace CinemaDirector
{
    /// <summary>
    /// A basic global action that can be Triggered when the Firetime is reached.
    /// </summary>
    [ExecuteInEditMode]
    public abstract class CinemaGlobalAction : TimelineAction, IComparable
    {
        /// <summary>
        /// Called when the running time of the cutscene hits the firetime of the action.
        /// </summary>
        /// <remarks>
        /// <c>Trigger()</c> is only called when the cutscene hits the firetime while moving forward, either through regular playback or by scrubbing. For when the playback is reversed, see <see cref="ReverseTrigger"/>.
        /// </remarks>
        public abstract void Trigger();

        /// <summary>
        /// Called at each update when the action is to be played.
        /// </summary>
        /// <param name="time">The new running time of the action.</param>
        /// <param name="deltaTime">The deltaTime since the last update call.</param>
        public virtual void UpdateTime(float time, float deltaTime) { }

        /// <summary>
        /// Called when the running time of the cutscene exceeds the duration of the action
        /// </summary>
        /// <remarks>
        /// <c>End()</c> is only called when the cutscene hits the endtime while moving forward, either through regular playback or by scrubbing. For when the playback is reversed, see <see cref="ReverseEnd"/>.
        /// </remarks>
        public abstract void End();

        /// <summary>
        /// Called when the cutscene time is set/skipped manually.
        /// </summary>
        /// <param name="time">The new running time of the action.</param>
        /// <param name="deltaTime">The deltaTime since the last update call.</param>
        public virtual void SetTime(float time, float deltaTime) { }

        /// <summary>
        /// Pause any action as necessary. Called when the cutscene is paused.
        /// </summary>
        public virtual void Pause() { }

        /// <summary>
        /// Resume from paused.  Called when the cutscene is unpaused.
        /// </summary>
        public virtual void Resume() { }

        /// <summary>
        /// Reverse trigger. Called when scrubbing backwards past the start of the action.
        /// </summary>
        public virtual void ReverseTrigger() { }

        /// <summary>
        /// Reverse End. Called when scrubbing backwards past the end of the action.
        /// </summary>
        public virtual void ReverseEnd() { }

        public int CompareTo(object other)
        {
            CinemaGlobalAction otherAction = (CinemaGlobalAction)other;
            return (int)(otherAction.Firetime - this.Firetime);
        }
    }
}