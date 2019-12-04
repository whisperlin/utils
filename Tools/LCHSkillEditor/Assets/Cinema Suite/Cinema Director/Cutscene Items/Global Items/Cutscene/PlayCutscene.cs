// Cinema Suite
using UnityEngine;

namespace CinemaDirector
{
    /// <summary>
    /// Play a given Cutscene.
    /// </summary>
    [CutsceneItem("Cutscene", "Play Cutscene", CutsceneItemGenre.GlobalItem)]
    public class PlayCutscene : CinemaGlobalEvent
    {
        // The Cutscene to be played.
        public Cutscene cutscene;

        /// <summary>
        /// Trigger this event and Play the given Cutscene.
        /// </summary>
        public override void Trigger()
        {
            if (cutscene != null)
            {
                cutscene.Play();
            }
        }
    }
}