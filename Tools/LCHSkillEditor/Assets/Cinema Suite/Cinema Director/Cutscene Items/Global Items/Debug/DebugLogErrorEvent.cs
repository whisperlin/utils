// Cinema Suite
using UnityEngine;

namespace CinemaDirector
{
    /// <summary>
    /// A simple event for debugging.
    /// </summary>
    [CutsceneItem("Debug", "Log Error", CutsceneItemGenre.GlobalItem)]
    public class DebugLogErrorEvent : CinemaGlobalEvent
    {
        // The message to be displayed.
        public string message = "Error Message";

        /// <summary>
        /// Trigger the Log Error Debug message.
        /// </summary>
        public override void Trigger()
        {
            Debug.LogError(message);
        }

        /// <summary>
        /// Trigger the Log Error message on reverse.
        /// </summary>
        public override void Reverse()
        {
            Debug.LogError(string.Format("Reverse: {0}", message));
        }
    }
}