// Cinema Suite
using UnityEngine;

namespace CinemaDirector
{
    /// <summary>
    /// Event to trigger playing an audio clip at a point in space.
    /// </summary>
    [CutsceneItemAttribute("Audio Source", "Play Clip At Point", CutsceneItemGenre.GlobalItem)]
    public class PlayClipAtPointAudioEvent : CinemaGlobalEvent
    {
        // The Audio Clip to be played.
        public AudioClip Clip;

        // The position to be played at.
        public Vector3 Position;

        // The volume.
        public float VolumeScale = 1f;

        /// <summary>
        /// Trigger the audio clip to be played at a position.
        /// </summary>
        public override void Trigger()
        {
            AudioSource.PlayClipAtPoint(Clip, Position, VolumeScale);
        }
    }
}