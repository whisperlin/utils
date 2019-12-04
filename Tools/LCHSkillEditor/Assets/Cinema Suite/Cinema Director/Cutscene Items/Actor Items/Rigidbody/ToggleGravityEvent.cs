using CinemaDirector.Helpers;
using System.Collections.Generic;
using UnityEngine;

namespace CinemaDirector
{
    /// <summary>
    /// An event to toggle the gravity of a given rigidbody.
    /// </summary>
    [CutsceneItemAttribute("Physics", "Toggle Gravity", CutsceneItemGenre.ActorItem)]
    public class ToggleGravityEvent : CinemaActorEvent, IRevertable
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
                    Rigidbody rb = go.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        reverts.Add(new RevertInfo(this, rb, "useGravity", rb.useGravity));
                    }
                }
            }

            return reverts.ToArray();
        }

        /// <summary>
        /// Toggle gravity for the given actor's rigidbody component.
        /// </summary>
        /// <param name="actor">The actor with a ridigbody to toggle gravity for.</param>
        public override void Trigger(GameObject actor)
        {
            Rigidbody affectedObjectRigidBody = actor.GetComponent<Rigidbody>();

            if (affectedObjectRigidBody != null)
            {
                affectedObjectRigidBody.useGravity = !affectedObjectRigidBody.useGravity;
            }
        }

        /// <summary>
        /// Reverse the toggle of gravity for the given actor's rigidbody component.
        /// </summary>
        /// <param name="actor">The actor with a ridigbody to toggle gravity for.</param>
        public override void Reverse(GameObject actor)
        {
            Trigger(actor);
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