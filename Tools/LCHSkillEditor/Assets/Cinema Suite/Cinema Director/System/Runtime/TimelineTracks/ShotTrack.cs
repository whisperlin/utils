// Cinema Suite
using System;
using UnityEngine;

namespace CinemaDirector
{
    public delegate void ShotBeginsEventHandler(object sender, ShotEventArgs e);
    public delegate void ShotEndsEventHandler(object sender, ShotEventArgs e);

    public class ShotEventArgs : EventArgs
    {
        public CinemaGlobalAction shot;

        public ShotEventArgs(CinemaGlobalAction shot)
        {
            this.shot = shot;
        }
    }

    /// <summary>
    /// A track that sorts shots and manages associated cameras.
    /// </summary>
    [TimelineTrackAttribute("Shot Track", TimelineTrackGenre.GlobalTrack, CutsceneItemGenre.CameraShot)]
    public class ShotTrack : TimelineTrack
    {
        public event ShotEndsEventHandler ShotEnds;
        public event ShotBeginsEventHandler ShotBegins;

        /// <summary>
        /// Initialize the shot track by enabling the first Camera and disabling all others in the track.
        /// </summary>
        public override void Initialize()
        {
            base.elapsedTime = 0f;

            CinemaGlobalAction firstCamera = null;
            TimelineItem[] items = GetTimelineItems();
            for (int i = 0; i < items.Length; i++)
            {
                CinemaGlobalAction shot = items[i] as CinemaGlobalAction;
                shot.Initialize();
            }

            for (int i = 0; i < items.Length; i++)
            {
                CinemaGlobalAction shot = items[i] as CinemaGlobalAction;
                if (shot.Firetime == 0)
                {
                    firstCamera = shot;
                }
                else
                {
                    shot.End();
                }
            }

            if (firstCamera != null)
            {
                firstCamera.Trigger();
                if (ShotBegins != null)
                {
                    ShotBegins(this, new ShotEventArgs(firstCamera));
                }
            }
        }

        /// <summary>
        /// Update the Shot Track by deltaTime. Will fire ShotBegins and ShotEnds events.
        /// </summary>
        /// <param name="time">The current running time.</param>
        /// <param name="deltaTime">The deltaTime since the last update.</param>
        public override void UpdateTrack(float time, float deltaTime)
        {
            float previousTime = base.elapsedTime;
            base.elapsedTime = time;

            TimelineItem[] items = GetTimelineItems();
            for (int i = 0; i < items.Length; i++)
            {
                CinemaGlobalAction shot = items[i] as CinemaGlobalAction;
                float endTime = shot.Firetime + shot.Duration;
                if ((previousTime <= shot.Firetime) && (base.elapsedTime >= shot.Firetime) && (base.elapsedTime < endTime))
                {
                    shot.Trigger();
                    if (ShotBegins != null)
                    {
                        ShotBegins(this, new ShotEventArgs(shot));
                    }
                }
                else if ((previousTime >= endTime) && (base.elapsedTime < endTime) && (base.elapsedTime >= shot.Firetime))
                {
                    shot.Trigger();
                    if (ShotBegins != null)
                    {
                        ShotBegins(this, new ShotEventArgs(shot));
                    }
                }
                else if ((previousTime >= shot.Firetime) && (previousTime < endTime) && (base.elapsedTime >= endTime))
                {
                    shot.End();
                    if (ShotEnds != null)
                    {
                        ShotEnds(this, new ShotEventArgs(shot));
                    }
                }
                else if ((previousTime > shot.Firetime) && (previousTime < endTime) && (base.elapsedTime < shot.Firetime))
                {
                    shot.End();
                    if (ShotEnds != null)
                    {
                        ShotEnds(this, new ShotEventArgs(shot));
                    }
                }
            }
        }

        /// <summary>
        /// The shot track will jump to the given time. Disabling the current shot and enabling the new one.
        /// </summary>
        /// <param name="time">The new running time.</param>
        public override void SetTime(float time)
        {
            CinemaGlobalAction previousShot = null;
            CinemaGlobalAction newShot = null;

            // Get the old shot and the new shot
            TimelineItem[] items = GetTimelineItems();
            for (int i = 0; i < items.Length; i++)
            {
                CinemaGlobalAction shot = items[i] as CinemaGlobalAction;
                float endTime = shot.Firetime + shot.Duration;
                if ((elapsedTime >= shot.Firetime) && (elapsedTime < endTime))
                {
                    previousShot = shot;
                }
                if ((time >= shot.Firetime) && (time < endTime))
                {
                    newShot = shot;
                }
            }

            // Trigger them as appropriate.
            if (newShot != previousShot)
            {
                if (previousShot != null)
                {
                    previousShot.End();
                    if (ShotEnds != null)
                    {
                        ShotEnds(this, new ShotEventArgs(previousShot));
                    }
                }
                if (newShot != null)
                {
                    newShot.Trigger();
                    if (ShotBegins != null)
                    {
                        ShotBegins(this, new ShotEventArgs(newShot));
                    }
                }
            }

            elapsedTime = time;
        }
    }
}