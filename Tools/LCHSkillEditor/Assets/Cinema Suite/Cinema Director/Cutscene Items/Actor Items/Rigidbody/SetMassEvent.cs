using CinemaDirector.Helpers;
using System.Collections.Generic;
using UnityEngine;

namespace CinemaDirector
{
    /// <summary>
    /// An event to set the mass of the rigidbody of a given actor.
    /// </summary>
    [CutsceneItemAttribute("Physics", "Set Mass", CutsceneItemGenre.ActorItem)]
    public class SetMassEvent : CinemaActorEvent, IRevertable
    {
        // The new mass.
        public float Mass = 1f;

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
                    Rigidbody rb = go.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        reverts.Add(new RevertInfo(this, rb, "mass", rb.mass));
                    }
                }
            }

            return reverts.ToArray();
        }

        /// <summary>
        /// Trigger this event and set a new mass for the actor's rigidbody.
        /// </summary>
        /// <param name="actor">The actor whose mass will be set.</param>
        public override void Trigger(GameObject actor)
        {
            if (actor == null) return;

            Rigidbody affectedObjectRigidBody = actor.GetComponent<Rigidbody>();

            if (affectedObjectRigidBody != null)
            {
                affectedObjectRigidBody.mass = Mass;
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