// Cinema Suite
using UnityEngine;

namespace CinemaDirector
{
    /// <summary>
    /// Pause a given Cutscene.
    /// </summary>
    [CutsceneItem("Cutscene", "Pause Cutscene", CutsceneItemGenre.GlobalItem)]
    public class PauseCutscene : CinemaGlobalEvent
    {
        // The cutscene to be paused.
        public Cutscene cutscene;

        /// <summary>
        /// Pause the given Cutscene.
        /// </summary>
        public override void Trigger()
        {
            if (cutscene != null)
            {
                cutscene.Pause();
            }
        }
    }
}