// Cinema Suite
using CinemaDirector.Helpers;
using UnityEngine;

namespace CinemaDirector
{
    /// <summary>
    /// An event for changing Time Scale.
    /// </summary>
    [CutsceneItem("Time", "Set Time Scale", CutsceneItemGenre.GlobalItem)]
    public class SetTimeScaleEvent : CinemaGlobalEvent, IRevertable
    {
        // The new timescale
        public float TimeScale = 1f;

        // Options for reverting in editor.
        [SerializeField]
        private RevertMode editorRevertMode = RevertMode.Revert;

        // Options for reverting during runtime.
        [SerializeField]
        private RevertMode runtimeRevertMode = RevertMode.Revert;

        // Keep track of the previous time scale when calling Trigger.
        private float previousTimescale = 1f;

        /// <summary>
        /// Cache the initial state of the time scale.
        /// </summary>
        /// <returns>The Info necessary to revert this event.</returns>
        public RevertInfo[] CacheState()
        {
            return new RevertInfo[] { new RevertInfo(this, typeof(Time), "timeScale", Time.timeScale) };
        }

        /// <summary>
        /// Trigger this event and set a new time scale.
        /// </summary>
        public override void Trigger()
        {
            previousTimescale = Time.timeScale;
            Time.timeScale = TimeScale;
        }

        /// <summary>
        /// Reverse this event and set time scale back to it's previous state.
        /// </summary>
        public override void Reverse()
        {
            Time.timeScale = previousTimescale;
        }

        /// <summary>
        /// Option for choosing when this Event will Revert to initial state in Editor.
        /// </summary>
        public RevertMode EditorRevertMode
        {
            get { return editorRevertMode; }
            set { editorRevertMode = value; }
        }

        /// <summary>
        /// Option for choosing when this Event will Revert to initial state in Runtime.
        /// </summary>
        public RevertMode RuntimeRevertMode
        {
            get { return runtimeRevertMode; }
            set { runtimeRevertMode = value; }
        }
    }
}