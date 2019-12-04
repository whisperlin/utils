using CinemaSuite.Common;
using System.Reflection;
using UnityEngine;

namespace CinemaDirector
{
    /// <summary>
    /// An Event for disabling any behaviour that has an "enabled" property.
    /// </summary>
    [CutsceneItemAttribute("Game Object", "Disable Behaviour", CutsceneItemGenre.ActorItem)]
    public class DisableBehaviour : CinemaActorEvent
    {
        public Component Behaviour;

        // Options for reverting in editor.
        [SerializeField]
        private RevertMode editorRevertMode = RevertMode.Revert;

        // Options for reverting during runtime.
        [SerializeField]
        private RevertMode runtimeRevertMode = RevertMode.Revert;

        /// <summary>
        /// Trigger this event and Disable the chosen Behaviour.
        /// </summary>
        /// <param name="actor">The actor to perform the behaviour disable on.</param>
        public override void Trigger(GameObject actor)
        {
            Component b = actor.GetComponent(Behaviour.GetType()) as Component;
            if (b != null)
            {
                PropertyInfo fieldInfo = ReflectionHelper.GetProperty(Behaviour.GetType(), "enabled");
                fieldInfo.SetValue(Behaviour, false, null);
            }
        }

        /// <summary>
        /// Reverse trigger this event and Enable the chosen Behaviour.
        /// </summary>
        /// <param name="actor">The actor to perform the behaviour enable on.</param>
        public override void Reverse(GameObject actor)
        {
            Component b = actor.GetComponent(Behaviour.GetType()) as Component;
            if (b != null)
            {
                PropertyInfo fieldInfo = ReflectionHelper.GetProperty(Behaviour.GetType(), "enabled");
                fieldInfo.SetValue(b, true, null);
            }
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