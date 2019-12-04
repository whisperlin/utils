using CinemaDirector.Helpers;
using System.Collections.Generic;
using UnityEngine;

namespace CinemaDirector
{
    /// <summary>
    /// Enable the Actor related to this event.
    /// </summary>
    [CutsceneItemAttribute("Game Object", "Temporary Enable", CutsceneItemGenre.ActorItem)]
    public class EnableGameObjectAction : CinemaActorAction, IRevertable
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
        /// <returns>All the revert info related to this event.</returns>
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
        /// Enable the given actor temporarily.
        /// </summary>
        /// <param name="actor">The actor to be enabled.</param>
        public override void Trigger(GameObject actor)
        {
            if (actor != null)
            {
                actor.SetActive(true);
            }
        }

        /// <summary>
        /// End the action and disable the game object.
        /// </summary>
        /// <param name="actor">The actor.</param>
        public override void End(GameObject actor)
        {
            if (actor != null)
            {
                actor.SetActive(false);
            }
        }

        /// <summary>
        /// The trigger time has been hit while playing in reverse. disable the action.
        /// </summary>
        /// <param name="Actor">The actor to disable</param>
        public override void ReverseTrigger(GameObject Actor)
        {
            this.End(Actor);
        }

        /// <summary>
        /// The end time has been hit while playing in reverse. enable the action.
        /// </summary>
        /// <param name="Actor">The actor to enable</param>
        public override void ReverseEnd(GameObject Actor)
        {
            Trigger(Actor);
        }

        /// <summary>
        /// Set the action to an arbitrary time.
        /// </summary>
        /// <param name="Actor">The current actor.</param>
        /// <param name="time">the running time of the action</param>
        /// <param name="deltaTime">The deltaTime since the last update call.</param>
        public override void SetTime(GameObject actor, float time, float deltaTime)
        {
            if (actor != null)
            {
                actor.SetActive(time >= 0 && time < Duration);
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