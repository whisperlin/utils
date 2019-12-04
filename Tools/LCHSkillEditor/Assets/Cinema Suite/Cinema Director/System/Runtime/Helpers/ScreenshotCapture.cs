using UnityEngine;

namespace CinemaDirector
{
    /// <summary>
    /// Behaviour for capturing screenshots at each frame.
    /// </summary>
    public class ScreenshotCapture : MonoBehaviour
    {
        public string Folder = "CaptureOutput";
        public int FrameRate = 24;

        void Start()
        {
            Time.captureFramerate = FrameRate;
            System.IO.Directory.CreateDirectory(Folder);
        }

        void Update()
        {
            string name = string.Format("{0}/shot {1:D04}.png", Folder, Time.frameCount);

            //ScreenCapture.CaptureScreenshot(name);
        }
    }
}