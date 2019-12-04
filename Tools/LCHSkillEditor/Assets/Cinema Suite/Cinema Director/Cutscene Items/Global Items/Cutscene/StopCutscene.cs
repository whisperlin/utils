// Cinema Suite
using UnityEngine;

namespace CinemaDirector
{
    /// <summary>
    /// Stop a given Cutscene from playing.
    /// </summary>
    [CutsceneItem("Cutscene", "Stop Cutscene", CutsceneItemGenre.GlobalItem)]
    public class StopCutscene : CinemaGlobalEvent
    {
        // The cutscene to be stopped.
        public Cutscene cutscene;

        /// <summary>
        /// Trigger this event and stop the given Cutscene from playing.
        /// </summary>
        public override void Trigger()
        {
            if (cutscene != null)
            {
                cutscene.Stop();
            }
        }
    }
}