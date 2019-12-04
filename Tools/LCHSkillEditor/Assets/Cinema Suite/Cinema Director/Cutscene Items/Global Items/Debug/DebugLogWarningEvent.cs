// Cinema Suite
using UnityEngine;

namespace CinemaDirector
{
    /// <summary>
    /// A simple event for logging warning messages to the console.
    /// </summary>
    [CutsceneItem("Debug", "Log Warning", CutsceneItemGenre.GlobalItem)]
    public class DebugLogWarningEvent : CinemaGlobalEvent
    {
        // The message to be displayed.
        public string message = "Warning Message";

        /// <summary>
        /// Trigger the log warning debug message.
        /// </summary>
        public override void Trigger()
        {
            Debug.LogWarning(message);
        }

        /// <summary>
        /// Trigger the Log warning message on reverse.
        /// </summary>
        public override void Reverse()
        {
            Debug.LogWarning(string.Format("Reverse: {0}", message));
        }
    }
}