using CinemaDirector.Helpers;
using System.Collections.Generic;
using UnityEngine;

namespace CinemaDirector
{
    /// <summary>
    /// An Actor event for disabling the Actor
    /// </summary>
    [CutsceneItemAttribute("Game Object", "Disable", CutsceneItemGenre.ActorItem)]
    public class DisableGameObject : CinemaActorEvent, IRevertable
    {
        // Options for reverting in editor.
        [SerializeField]
        private RevertMode editorRevertMode = RevertMode.Revert;

        // Options for reverting during runtime.
        [SerializeField]
        private RevertMode runtimeRevertMode = RevertMode.Revert;

        /// <summary>
        /// Cache the state of all actors related to this event.
        /// </summary>
        /// <returns></returns>
        public RevertInfo[] CacheState()
        {
            List<Transform> actors = new List<Transform>(GetActors());
            List<RevertInfo> reverts = new List<RevertInfo>();
            for (int i = 0; i < actors.Count; i++)
            {
                Transform go = actors[i];
                if (go != null)
                {
                    reverts.Add(new RevertInfo(this, go.gameObject, "SetActive", go.gameObject.activeSelf));
                }
            }

            return reverts.ToArray();
        }

        /// <summary>
        /// Trigger this event and disable the given actor.
        /// </summary>
        /// <param name="actor">The actor to be disabled.</param>
        public override void Trigger(GameObject actor)
        {
            if (actor != null)
            {
                actor.SetActive(false);
            }
        }

        /// <summary>
        /// Reverse the event by settings the game object to the previous state.
        /// </summary>
        /// <param name="actor">The actor whose active state will be reversed.</param>
        public override void Reverse(GameObject actor)
        {
            if (actor != null)
            {
                actor.SetActive(true); // ToDo: Implement reversing with cacheing previous state.
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