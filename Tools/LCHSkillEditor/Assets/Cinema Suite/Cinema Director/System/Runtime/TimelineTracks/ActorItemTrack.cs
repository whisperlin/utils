using System;
// Cinema Suite
using System.Collections.Generic;
using UnityEngine;

namespace CinemaDirector
{
    /// <summary>
    /// A track which maintains all timeline items marked for actor tracks and multi actor tracks.
    /// </summary>
    [TimelineTrackAttribute("Actor Track", new TimelineTrackGenre[] { TimelineTrackGenre.ActorTrack, TimelineTrackGenre.MultiActorTrack }, CutsceneItemGenre.ActorItem)]
    public class ActorItemTrack : TimelineTrack, IActorTrack, IMultiActorTrack
    {
        /// <summary>
        /// Initialize this Track and all the timeline items contained within.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            for (int i = 0; i < this.ActorEvents.Length; i++)
            {
                for (int j = 0; j < Actors.Count; j++)
                {
                    if (Actors[j] != null)
                    {
                        this.ActorEvents[i].Initialize(Actors[j].gameObject);
                    }
                }
            }
        }

        /// <summary>
        /// The cutscene has been set to an arbitrary time by the user.
        /// Processing must take place to catch up to the new time.
        /// </summary>
        /// <param name="time">The new cutscene running time</param>
        public override void SetTime(float time)
        {
            float previousTime = elapsedTime;
            base.SetTime(time);

            TimelineItem[] items = GetTimelineItems();
            for (int i = 0; i < items.Length; i++)
            {
                // Check if it is an actor event.
                CinemaActorEvent cinemaEvent = items[i] as CinemaActorEvent;
                if (cinemaEvent != null)
                {
                    if ((previousTime < cinemaEvent.Firetime && time >= cinemaEvent.Firetime) || (cinemaEvent.Firetime == 0f && previousTime <= cinemaEvent.Firetime && time > cinemaEvent.Firetime))
                    {
                        for (int j = 0; j < Actors.Count; j++)
                        {
                            if (Actors[j] != null)
                            {
                                cinemaEvent.Trigger(Actors[j].gameObject);
                            }
                        }
                    }
                    else if (previousTime > cinemaEvent.Firetime && time <= cinemaEvent.Firetime)
                    {
                        for (int j = 0; j < Actors.Count; j++)
                        {
                            if (Actors[j] != null)
                                cinemaEvent.Reverse(Actors[j].gameObject);
                        }
                    }
                }

                // Check if it is an actor action.
                CinemaActorAction action = items[i] as CinemaActorAction;
                if (action != null)
                {
                    for (int j = 0; j < Actors.Count; j++)
                    {
                        if (Actors[j] != null)
                        {
                                action.SetTime(Actors[j].gameObject, (time - action.Firetime), time - previousTime);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Update this track since the last frame.
        /// </summary>
        /// <param name="time">The new running time.</param>
        /// <param name="deltaTime">The deltaTime since last update.</param>
        public override void UpdateTrack(float time, float deltaTime)
        {
            float previousTime = base.elapsedTime;
            base.UpdateTrack(time, deltaTime);

            TimelineItem[] items = GetTimelineItems();
            for (int i = 0; i < items.Length; i++)
            {
                // Check if it is an actor event.
                CinemaActorEvent cinemaEvent = items[i] as CinemaActorEvent;
                if (cinemaEvent != null)
                {
                    if ((previousTime < cinemaEvent.Firetime && time >= cinemaEvent.Firetime) || (cinemaEvent.Firetime == 0f && previousTime <= cinemaEvent.Firetime && time > cinemaEvent.Firetime))
                    {
                        for (int j = 0; j < Actors.Count; j++)
                        {
                            if (Actors[j] != null)
                                cinemaEvent.Trigger(Actors[j].gameObject);
                        }
                    }
                    else if (previousTime >= cinemaEvent.Firetime && base.elapsedTime <= cinemaEvent.Firetime)
                    {
                        for (int j = 0; j < Actors.Count; j++)
                        {
                            if (Actors[j] != null)
                                cinemaEvent.Reverse(Actors[j].gameObject);
                        }
                    }
                }

                CinemaActorAction action = items[i] as CinemaActorAction;
                if (action != null)
                {
                    if (((previousTime < action.Firetime || previousTime <= 0f) && base.elapsedTime >= action.Firetime) && base.elapsedTime < action.EndTime)
                    {
                        for (int j = 0; j < Actors.Count; j++)
                        {
                            if (Actors[j] != null)
                            {
                                action.Trigger(Actors[j].gameObject);
                            }
                        }
                    }
                    else if (previousTime < action.EndTime && base.elapsedTime >= action.EndTime)
                    {
                        for (int j = 0; j < Actors.Count; j++)
                        {
                            if (Actors[j] != null)
                            {
                                action.End(Actors[j].gameObject);
                            }
                        }
                    }
                    else if (previousTime >= action.Firetime && previousTime < action.EndTime && base.elapsedTime <= action.Firetime)
                    {
                        for (int j = 0; j < Actors.Count; j++)
                        {
                            if (Actors[j] != null)
                            {
                                action.ReverseTrigger(Actors[j].gameObject);
                            }
                        }
                    }
                    else if (((previousTime > action.EndTime || previousTime >= action.Cutscene.Duration) && (base.elapsedTime > action.Firetime) && (base.elapsedTime <= action.EndTime)))
                    {
                        for (int j = 0; j < Actors.Count; j++)
                        {
                            if (Actors[j] != null)
                            {
                                action.ReverseEnd(Actors[j].gameObject);
                            }
                        }
                    }
                    else if ((base.elapsedTime > action.Firetime) && (base.elapsedTime <= action.EndTime))
                    {
                        for (int j = 0; j < Actors.Count; j++)
                        {
                            if (Actors[j] != null)
                            {
                                float runningTime = time - action.Firetime;
                                action.UpdateTime(Actors[j].gameObject, runningTime, deltaTime);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Pause playback while being played.
        /// </summary>
        public override void Pause()
        {
            base.Pause();
            TimelineItem[] items = GetTimelineItems();
            for (int i = 0; i < items.Length; i++)
            {
                CinemaActorAction action = items[i] as CinemaActorAction;
                if (action != null)
                {
                    if (((elapsedTime > action.Firetime)) && (elapsedTime < (action.Firetime + action.Duration)))
                    {
                        for (int j = 0; j < Actors.Count; j++)
                        {
                            if (Actors[j] != null)
                            {
                                action.Pause(Actors[j].gameObject);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Resume playback after being paused.
        /// </summary>
        public override void Resume()
        {
            base.Resume();
            TimelineItem[] items = GetTimelineItems();
            for (int i = 0; i < items.Length; i++)
            {
                CinemaActorAction action = items[i] as CinemaActorAction;
                if (action != null)
                {
                    if (((elapsedTime > action.Firetime)) && (elapsedTime < (action.Firetime + action.Duration)))
                    {
                        for (int j = 0; j < Actors.Count; j++)
                        {
                            if (Actors[j] != null)
                            {
                                action.Resume(Actors[j].gameObject);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Stop the playback of this track.
        /// </summary>
        public override void Stop()
        {
            base.Stop();
            base.elapsedTime = 0f;
            TimelineItem[] items = GetTimelineItems();
            for (int i = 0; i < items.Length; i++)
            {
                CinemaActorEvent cinemaEvent = items[i] as CinemaActorEvent;
                if (cinemaEvent != null)
                {
                    for (int j = 0; j < Actors.Count; j++)
                    {
                        if (Actors[j] != null)
                            cinemaEvent.Stop(Actors[j].gameObject);
                    }
                }
            
                CinemaActorAction action = items[i] as CinemaActorAction;
                if (action != null)
                {
                    for (int j = 0; j < Actors.Count; j++)
                    {
                        if (Actors[j] != null)
                            action.Stop(Actors[j].gameObject);
                    }
                }
            }
        }

        /// <summary>
        /// Get the Actor associated with this track. Can return null.
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

        /// <summary>
        /// Get the Actors associated with this track. Can return null.
        /// In the case of MultiActors it will return the full list.
        /// </summary>
        public List<Transform> Actors
        {
            get
            {
                ActorTrackGroup trackGroup = TrackGroup as ActorTrackGroup;
                if (trackGroup != null)
                {
                    List<Transform> actors = new List<Transform>() { };
                    actors.Add(trackGroup.Actor);
                    return actors;
                }

                MultiActorTrackGroup multiActorTrackGroup = TrackGroup as MultiActorTrackGroup;
                if (multiActorTrackGroup != null)
                {
                    return multiActorTrackGroup.Actors;
                }
                return null;
            }
        }

        public CinemaActorEvent[] ActorEvents
        {
            get
            {
                return base.GetComponentsInChildren<CinemaActorEvent>();
            }
        }

        public CinemaActorAction[] ActorActions
        {
            get
            {
                return base.GetComponentsInChildren<CinemaActorAction>();
            }
        }
    }
}