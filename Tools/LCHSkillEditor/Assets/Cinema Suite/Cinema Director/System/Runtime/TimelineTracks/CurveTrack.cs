using UnityEngine;

namespace CinemaDirector
{
    /// <summary>
    /// A track designed to hold Actor Curve Clip items.
    /// </summary>
    [TimelineTrackAttribute("Curve Track", TimelineTrackGenre.ActorTrack, CutsceneItemGenre.CurveClipItem)]
    public class CurveTrack : TimelineTrack, IActorTrack
    {
        /// <summary>
        /// Update all curve items.
        /// </summary>
        /// <param name="time">The new running time.</param>
        /// <param name="deltaTime">The deltaTime since last update.</param>
        public override void UpdateTrack(float time, float deltaTime)
        {
            base.elapsedTime = time;

            TimelineItem[] items = GetTimelineItems();
            for (int i = 0; i < items.Length; i++)
            {
                CinemaActorClipCurve actorClipCurve = items[i] as CinemaActorClipCurve;
                if (actorClipCurve != null)
                {
                    actorClipCurve.SampleTime(time);
                }
            }
        }

        /// <summary>
        /// Set the track to an arbitrary time.
        /// </summary>
        /// <param name="time">The new running time.</param>
        public override void SetTime(float time)
        {
            base.elapsedTime = time;
            TimelineItem[] items = GetTimelineItems();
            for (int i = 0; i < items.Length; i++)
            {
                CinemaActorClipCurve actorClipCurve = items[i] as CinemaActorClipCurve;
                if (actorClipCurve != null)
                {
                    actorClipCurve.SampleTime(time);
                }
            }
        }

        /// <summary>
        /// Stop and reset all the curve data.
        /// </summary>
        public override void Stop()
        {
            TimelineItem[] items = GetTimelineItems();
            for (int i = 0; i < items.Length; i++)
            {
                CinemaActorClipCurve actorClipCurve = items[i] as CinemaActorClipCurve;
                if (actorClipCurve != null)
                {
                    actorClipCurve.Reset();
                }
            }
        }

        /// <summary>
        /// Get the Actor associated with this Curve Track.
        /// </summary>
        public Transform Actor
        {
            get
            {
                ActorTrackGroup atg = this.TrackGroup as ActorTrackGroup;
                if (atg == null)
                {
                    Debug.LogError("No ActorTrackGroup found on parent.", this);
                    return null;
                }
                return atg.Actor;
            }
        }
    }
}