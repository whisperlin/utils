
namespace CinemaDirector
{
    /// <summary>
    /// A track designed specifically to hold audio items.
    /// </summary>
    [TimelineTrackAttribute("Audio Track", TimelineTrackGenre.GlobalTrack, CutsceneItemGenre.AudioClipItem)]
    public class AudioTrack : TimelineTrack
    {
        /// <summary>
        /// Set the track to an arbitrary time.
        /// </summary>
        /// <param name="time">The new time.</param>
        public override void SetTime(float time)
        {
            TimelineItem[] items = GetTimelineItems();
            for (int i = 0; i < items.Length; i++)
            {
                CinemaAudio cinemaAudio = items[i] as CinemaAudio;
                if (cinemaAudio != null)
                {
                    float audioTime = time - cinemaAudio.Firetime;
                    cinemaAudio.SetTime(audioTime);
                }
            }
        }

        /// <summary>
        /// Pause all Audio Clips that are currently playing.
        /// </summary>
        public override void Pause()
        {
            TimelineItem[] items = GetTimelineItems();
            for (int i = 0; i < items.Length; i++)
            {
                CinemaAudio cinemaAudio = items[i] as CinemaAudio;
                if (cinemaAudio != null)
                {
                    cinemaAudio.Pause();
                }
            }
        }

        /// <summary>
        /// Update the track and play any newly triggered items.
        /// </summary>
        /// <param name="time">The new running time.</param>
        /// <param name="deltaTime">The deltaTime since the last update call.</param>
        public override void UpdateTrack(float time, float deltaTime)
        {
            float elapsedTime = base.elapsedTime;
            base.elapsedTime = time;

            TimelineItem[] items = GetTimelineItems();
            for (int i = 0; i < items.Length; i++)
            {
                CinemaAudio cinemaAudio = items[i] as CinemaAudio;
                if (cinemaAudio != null)
                {
                    if (((elapsedTime < cinemaAudio.Firetime) || (elapsedTime <= 0f)) && (((base.elapsedTime >= cinemaAudio.Firetime))))
                    {
                        cinemaAudio.Trigger();
                    }
                    if ((base.elapsedTime > cinemaAudio.Firetime) && (base.elapsedTime <= (cinemaAudio.Firetime + cinemaAudio.Duration)))
                    {
                        float audioTime = time - cinemaAudio.Firetime;
                        cinemaAudio.UpdateTime(audioTime, deltaTime);
                    }
                    if (((elapsedTime <= (cinemaAudio.Firetime + cinemaAudio.Duration)) && (base.elapsedTime > (cinemaAudio.Firetime + cinemaAudio.Duration))))
                    {
                        cinemaAudio.End();
                    }
                }
            }
        }

        /// <summary>
        /// Resume playing audio clips after calling a Pause.
        /// </summary>
        public override void Resume()
        {
            TimelineItem[] items = GetTimelineItems();
            for (int i = 0; i < items.Length; i++)
            {
                CinemaAudio cinemaAudio = items[i] as CinemaAudio;
                if (cinemaAudio != null)
                {
                    if (((base.Cutscene.RunningTime > cinemaAudio.Firetime)) && (base.Cutscene.RunningTime < (cinemaAudio.Firetime + cinemaAudio.Duration)))
                    {
                        cinemaAudio.Resume();
                    }
                }
            }
        }

        /// <summary>
        /// Stop playback of all playing audio items.
        /// </summary>
        public override void Stop()
        {
            base.elapsedTime = 0f;
            TimelineItem[] items = GetTimelineItems();
            for (int i = 0; i < items.Length; i++)
            {
                CinemaAudio cinemaAudio = items[i] as CinemaAudio;
                if (cinemaAudio != null)
                {
                    cinemaAudio.Stop();
                }
            }
        }

        /// <summary>
        /// Get all cinema audio objects associated with this audio track
        /// </summary>
        public CinemaAudio[] AudioClips
        {
            get
            {
                return GetComponentsInChildren<CinemaAudio>();
            }
        }
    }
}