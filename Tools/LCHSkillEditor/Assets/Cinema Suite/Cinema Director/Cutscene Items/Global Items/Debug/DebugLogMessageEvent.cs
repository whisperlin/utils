// Cinema Suite
using UnityEngine;

namespace CinemaDirector
{
    /// <summary>
    /// A simple event for logging messages to the console.
    /// </summary>
    [CutsceneItem("Debug", "Log Message", CutsceneItemGenre.GlobalItem)]
    public class DebugLogMessageEvent : CinemaGlobalEvent
    {
        // The message to be displayed.
        public string message = "Log Message";

        /// <summary>
        /// Trigger the log debug message.
        /// </summary>
        public override void Trigger()
        {
            Debug.Log(message);
        }

        /// <summary>
        /// Trigger the Log message on reverse.
        /// </summary>
        public override void Reverse()
        {
            Debug.Log(string.Format("Reverse: {0}", message));
        }
    }
}