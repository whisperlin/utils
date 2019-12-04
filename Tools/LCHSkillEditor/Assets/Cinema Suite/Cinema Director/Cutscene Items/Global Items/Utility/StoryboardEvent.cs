// Cinema Suite
using UnityEngine;

namespace CinemaDirector
{
    /// <summary>
    /// Event that captures a screenshot when triggered.
    /// </summary>
    [CutsceneItem("Utility", "Storyboard", CutsceneItemGenre.GlobalItem)]
    public class StoryboardEvent : CinemaGlobalEvent
    {
        public string FolderName = "Storyboard";
        public static int Count = 0; // Count how many screenshots have been captured.

        /// <summary>
        /// Capture screenshot on trigger.
        /// </summary>
        public override void Trigger()
        {
            //ScreenCapture.CaptureScreenshot(string.Format(@"Assets\{0}{1}.png", this.gameObject.name, Count++));
        }
    }
}